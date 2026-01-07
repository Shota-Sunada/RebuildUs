package main

import (
	"archive/zip"
	"bufio"
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"regexp"
	"sort"
	"strconv"
	"strings"
)

type GitHubCommit struct {
	SHA    string `json:"sha"`
	Commit struct {
		Message string `json:"message"`
	} `json:"commit"`
}

type GitHubRelease struct {
	Assets []struct {
		Name               string `json:"name"`
		BrowserDownloadURL string `json:"browser_download_url"`
	} `json:"assets"`
}

func main() {
	fmt.Println("Rebuild-Us Release Zip Generator")
	fmt.Println("================================")

	version := ""
	if len(os.Args) > 1 {
		version = os.Args[1]
	}

	fmt.Println("Fetching BepInEx builds...")
	builds, err := getBepInExBuilds()
	if err != nil || len(builds) == 0 {
		fmt.Printf("Failed to fetch builds: %v\n", err)
		return
	}

	fmt.Println("Fetching commits from GitHub...")
	commits, err := getGitHubCommits("BepInEx", "BepInEx")
	if err != nil {
		fmt.Printf("Warning: Failed to fetch GitHub commits: %v\n", err)
	}

	type DisplayItem struct {
		BuildID string
		Hash    string
		Message string
	}
	var displayList []DisplayItem

	var keys []int
	for k := range builds {
		id, _ := strconv.Atoi(k)
		keys = append(keys, id)
	}
	sort.Sort(sort.Reverse(sort.IntSlice(keys)))

	count := 0
	for _, idInt := range keys {
		if count >= 15 {
			break
		}
		id := strconv.Itoa(idInt)
		hash := builds[id]
		message := "Unknown commit"

		for _, c := range commits {
			if strings.HasPrefix(c.SHA, hash) {
				message = strings.Split(c.Commit.Message, "\n")[0]
				break
			}
		}

		displayList = append(displayList, DisplayItem{id, hash, message})
		fmt.Printf("[%d] #%s (%s): %s\n", count, id, hash, message)
		count++
	}

	fmt.Print("\nEnter BepInEx build number (e.g. 752) or index: ")
	reader := bufio.NewReader(os.Stdin)
	input, _ := reader.ReadString('\n')
	input = strings.TrimSpace(input)

	var buildNumber, commitHash string

	if hash, ok := builds[input]; ok {
		buildNumber = input
		commitHash = hash
	} else if index, err := strconv.Atoi(input); err == nil && index >= 0 && index < len(displayList) {
		buildNumber = displayList[index].BuildID
		commitHash = displayList[index].Hash
	} else if matched, _ := regexp.MatchString(`^\d+$`, input); matched {
		buildNumber = input
		commitHash, _ = getHashForBuild(buildNumber)
	}

	if buildNumber == "" || commitHash == "" {
		fmt.Println("Invalid input or could not find build hash.")
		return
	}

	// Download Submerged.dll
	fmt.Println("\nFetching Submerged.dll from GitHub...")
	submergedUrl, err := getGitHubLatestReleaseAsset("SubmergedAmongUs", "Submerged", "Submerged.dll")
	submergedDllPath := ""
	if err == nil {
		fmt.Printf("Downloading Submerged.dll: %s\n", submergedUrl)
		submergedDllPath = filepath.Join(os.TempDir(), "Submerged.dll")
		if err := downloadFile(submergedUrl, submergedDllPath); err != nil {
			fmt.Printf("Warning: Failed to download Submerged.dll: %v\n", err)
			submergedDllPath = ""
		} else {
			defer os.Remove(submergedDllPath)
		}
	} else {
		fmt.Printf("Warning: Could not find Submerged.dll release: %v\n", err)
	}

	// Download Reactor.dll
	fmt.Println("\nFetching Reactor.dll from GitHub...")
	reactorUrl, err := getGitHubLatestReleaseAsset("NuclearPowered", "Reactor", "Reactor.dll")
	reactorDllPath := ""
	if err == nil {
		fmt.Printf("Downloading Reactor.dll: %s\n", reactorUrl)
		reactorDllPath = filepath.Join(os.TempDir(), "Reactor.dll")
		if err := downloadFile(reactorUrl, reactorDllPath); err != nil {
			fmt.Printf("Warning: Failed to download Reactor.dll: %v\n", err)
			reactorDllPath = ""
		} else {
			defer os.Remove(reactorDllPath)
		}
	} else {
		fmt.Printf("Warning: Could not find Reactor.dll release: %v\n", err)
	}

	// 1. Generate for Steam/Itch (x86) - No Submerged
	fmt.Println("\n--- Generating for Steam/Itch (x86) ---")
	if err := generateZip(version, buildNumber, commitHash, "x86", "Steam-Itch", "[32bit] STEAM_ITCH", "", ""); err != nil {
		fmt.Printf("Failed to generate Steam/Itch zip: %v\n", err)
	}

	// 2. Generate for Epic/MS Store (x64) - No Submerged
	fmt.Println("\n--- Generating for Epic/MS Store (x64) ---")
	if err := generateZip(version, buildNumber, commitHash, "x64", "Epic-MSStore", "[64bit] EPIC_MSSTORE", "", ""); err != nil {
		fmt.Printf("Failed to generate Epic/MS Store zip: %v\n", err)
	}

	if submergedDllPath != "" {
		// 3. Generate for Steam/Itch (x86) - With Submerged
		fmt.Println("\n--- Generating for Steam/Itch (x86) with Submerged ---")
		if err := generateZip(version, buildNumber, commitHash, "x86", "Steam-Itch-Submerged", "[32bit] STEAM_ITCH (Submerged)", submergedDllPath, reactorDllPath); err != nil {
			fmt.Printf("Failed to generate Steam/Itch Submerged zip: %v\n", err)
		}

		// 4. Generate for Epic/MS Store (x64) - With Submerged
		fmt.Println("\n--- Generating for Epic/MS Store (x64) with Submerged ---")
		if err := generateZip(version, buildNumber, commitHash, "x64", "Epic-MSStore-Submerged", "[64bit] EPIC_MSSTORE (Submerged)", submergedDllPath, reactorDllPath); err != nil {
			fmt.Printf("Failed to generate Epic/MS Store Submerged zip: %v\n", err)
		}
	}

	fmt.Println("\nAll release zips created successfully!")
}

func generateZip(version, buildNumber, commitHash, arch, platformLabel, markerFileName, submergedDllPath, reactorDllPath string) error {
	bepInExUrl := fmt.Sprintf("https://builds.bepinex.dev/projects/bepinex_be/%s/BepInEx-Unity.IL2CPP-win-%s-6.0.0-be.%s%%2B%s.zip", buildNumber, arch, buildNumber, commitHash)
	fmt.Printf("Downloading BepInEx (%s): %s\n", arch, bepInExUrl)

	tempBepInExZip := filepath.Join(os.TempDir(), fmt.Sprintf("BepInEx_%s_%s.zip", buildNumber, arch))
	if err := downloadFile(bepInExUrl, tempBepInExZip); err != nil {
		return fmt.Errorf("download failed: %v", err)
	}
	defer os.Remove(tempBepInExZip)

	zipName := fmt.Sprintf("RebuildUs-Release-%s.zip", platformLabel)
	if version != "" {
		zipName = fmt.Sprintf("RebuildUs-v%s-%s.zip", version, platformLabel)
	}
	outputZipPath := findOutputPath(zipName)

	// Ensure Release directory exists if we are putting it there
	if dir := filepath.Dir(outputZipPath); dir != "." {
		os.MkdirAll(dir, 0755)
	}

	fmt.Printf("Creating %s...\n", outputZipPath)

	newZipFile, err := os.Create(outputZipPath)
	if err != nil {
		return fmt.Errorf("failed to create output zip: %v", err)
	}
	defer newZipFile.Close()

	zipWriter := zip.NewWriter(newZipFile)
	defer zipWriter.Close()

	// 1. Copy BepInEx contents to root
	fmt.Println("Adding BepInEx files...")
	if err := copyZipToZip(tempBepInExZip, zipWriter); err != nil {
		return fmt.Errorf("failed to copy BepInEx files: %v", err)
	}

	// 2. Add steam_appid.txt (Only for Steam/Itch)
	if platformLabel == "Steam-Itch" {
		fmt.Println("Adding steam_appid.txt...")
		if err := addStringToZip(zipWriter, "steam_appid.txt", "945360"); err != nil {
			return fmt.Errorf("failed to add steam_appid.txt: %v", err)
		}
	}

	// 2.5 Add Marker File
	fmt.Printf("Adding marker file: %s...\n", markerFileName)
	if err := addStringToZip(zipWriter, markerFileName, ""); err != nil {
		return fmt.Errorf("failed to add marker file: %v", err)
	}

	// 3. Add LICENSE
	fmt.Println("Adding LICENSE...")
	licensePath := findFile("LICENSE")
	if licensePath != "" {
		if err := addFileToZip(zipWriter, licensePath, "LICENSE"); err != nil {
			fmt.Printf("Failed to add LICENSE: %v\n", err)
		}
	} else {
		fmt.Println("Warning: LICENSE file not found.")
	}

	// 3.5 Add Release-Readme.txt
	fmt.Println("Adding Release-Readme.txt...")
	readmePath := findFile("Release-Readme.txt")
	if readmePath != "" {
		if err := addFileToZip(zipWriter, readmePath, "README.txt"); err != nil {
			fmt.Printf("Failed to add Release-Readme.txt: %v\n", err)
		}
	} else {
		fmt.Println("Warning: Release-Readme.txt file not found.")
	}

	// 4. Add RebuildUs.dll
	fmt.Println("Adding RebuildUs.dll...")
	dllPath := findFile("RebuildUs/bin/Release/net6.0/RebuildUs.dll")
	if dllPath != "" {
		if err := addFileToZip(zipWriter, dllPath, "BepInEx/plugins/RebuildUs.dll"); err != nil {
			fmt.Printf("Failed to add RebuildUs.dll: %v\n", err)
		}
	} else {
		fmt.Println("Warning: RebuildUs.dll not found. Make sure to build in Release mode first.")
	}

	// 5. Add BepInExUpdater.exe
	fmt.Println("Adding BepInExUpdater.exe...")
	updaterPath := findFile("BepInExUpdater/publish/BepInExUpdater.exe")
	if updaterPath != "" {
		if err := addFileToZip(zipWriter, updaterPath, "BepInExUpdater.exe"); err != nil {
			fmt.Printf("Failed to add BepInExUpdater.exe: %v\n", err)
		}
	} else {
		fmt.Println("Warning: BepInExUpdater.exe not found.")
	}

	// 6. Add CosmeticsDownloader.exe
	fmt.Println("Adding CosmeticsDownloader.exe...")
	cosmeticsDownloaderPath := findFile("RebuildUs/bin/Release/net6.0/CosmeticsDownloader.exe")
	if cosmeticsDownloaderPath != "" {
		if err := addFileToZip(zipWriter, cosmeticsDownloaderPath, "BepInEx/plugins/CosmeticsDownloader.exe"); err != nil {
			fmt.Printf("Failed to add CosmeticsDownloader.exe: %v\n", err)
		}
	} else {
		fmt.Println("Warning: CosmeticsDownloader.exe not found.")
	}

	// 6. Add Submerged.dll if provided
	if submergedDllPath != "" {
		fmt.Println("Adding Submerged.dll...")
		if err := addFileToZip(zipWriter, submergedDllPath, "BepInEx/plugins/Submerged.dll"); err != nil {
			fmt.Printf("Failed to add Submerged.dll: %v\n", err)
		}
	}

	// 7. Add Reactor.dll if provided
	if reactorDllPath != "" {
		fmt.Println("Adding Reactor.dll...")
		if err := addFileToZip(zipWriter, reactorDllPath, "BepInEx/plugins/Reactor.dll"); err != nil {
			fmt.Printf("Failed to add Reactor.dll: %v\n", err)
		}
	}

	return nil
}

func findFile(name string) string {
	if _, err := os.Stat(name); err == nil {
		return name
	}
	// Try parent directory (if running from ReleaseZipGenerator or ReleaseZipGenerator/publish)
	path := filepath.Join("..", name)
	if _, err := os.Stat(path); err == nil {
		return path
	}
	// Try two levels up (if running from ReleaseZipGenerator/publish)
	path = filepath.Join("..", "..", name)
	if _, err := os.Stat(path); err == nil {
		return path
	}
	return ""
}

func findOutputPath(name string) string {
	// If we are in ReleaseZipGenerator, we want to put it in the root's Release folder
	if _, err := os.Stat("go.mod"); err == nil {
		cwd, _ := os.Getwd()
		if strings.HasSuffix(cwd, "ReleaseZipGenerator") {
			return filepath.Join("..", "Release", name)
		}
	}
	return filepath.Join("Release", name)
}

func getBepInExBuilds() (map[string]string, error) {
	resp, err := http.Get("https://builds.bepinex.dev/projects/bepinex_be")
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return nil, err
	}
	html := string(body)

	builds := make(map[string]string)
	re := regexp.MustCompile(`/projects/bepinex_be/(\d+)/BepInEx.*?be\.\d+%2B([a-f0-9]{7})\.zip`)
	matches := re.FindAllStringSubmatch(html, -1)

	for _, m := range matches {
		builds[m[1]] = m[2]
	}

	if len(builds) == 0 {
		re2 := regexp.MustCompile(`#(\d+).*?([a-f0-9]{7})`)
		matches2 := re2.FindAllStringSubmatch(html, -1)
		for _, m := range matches2 {
			if len(m[1]) >= 3 {
				builds[m[1]] = m[2]
			}
		}
	}

	return builds, nil
}

func getHashForBuild(buildNumber string) (string, error) {
	url := fmt.Sprintf("https://builds.bepinex.dev/projects/bepinex_be/%s", buildNumber)
	resp, err := http.Get(url)
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(resp.Body)
	if err != nil {
		return "", err
	}

	re := regexp.MustCompile(`<code>([a-f0-9]{7})</code>`)
	match := re.FindStringSubmatch(string(body))
	if len(match) > 1 {
		return match[1], nil
	}
	return "", fmt.Errorf("hash not found")
}

func getGitHubCommits(owner, repo string) ([]GitHubCommit, error) {
	url := fmt.Sprintf("https://api.github.com/repos/%s/%s/commits", owner, repo)
	req, _ := http.NewRequest("GET", url, nil)
	req.Header.Set("User-Agent", "ReleaseZipGenerator")

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return nil, err
	}
	defer resp.Body.Close()

	if resp.StatusCode == http.StatusForbidden {
		return nil, fmt.Errorf("GitHub API rate limit exceeded")
	}

	var commits []GitHubCommit
	if err := json.NewDecoder(resp.Body).Decode(&commits); err != nil {
		return nil, err
	}
	return commits, nil
}

func getGitHubLatestReleaseAsset(owner, repo, assetName string) (string, error) {
	url := fmt.Sprintf("https://api.github.com/repos/%s/%s/releases/latest", owner, repo)
	req, _ := http.NewRequest("GET", url, nil)
	req.Header.Set("User-Agent", "ReleaseZipGenerator")

	resp, err := http.DefaultClient.Do(req)
	if err != nil {
		return "", err
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return "", fmt.Errorf("failed to fetch latest release: %s", resp.Status)
	}

	var release GitHubRelease
	if err := json.NewDecoder(resp.Body).Decode(&release); err != nil {
		return "", err
	}

	for _, asset := range release.Assets {
		if asset.Name == assetName {
			return asset.BrowserDownloadURL, nil
		}
	}

	return "", fmt.Errorf("%s not found in latest release", assetName)
}

func downloadFile(url, outputPath string) error {
	resp, err := http.Get(url)
	if err != nil {
		return err
	}
	defer resp.Body.Close()

	if resp.StatusCode != http.StatusOK {
		return fmt.Errorf("bad status: %s", resp.Status)
	}

	out, err := os.Create(outputPath)
	if err != nil {
		return err
	}
	defer out.Close()

	_, err = io.Copy(out, resp.Body)
	return err
}

func copyZipToZip(srcZipPath string, zw *zip.Writer) error {
	r, err := zip.OpenReader(srcZipPath)
	if err != nil {
		return err
	}
	defer r.Close()

	for _, f := range r.File {
		if f.FileInfo().IsDir() {
			continue
		}

		rc, err := f.Open()
		if err != nil {
			return err
		}

		w, err := zw.Create(f.Name)
		if err != nil {
			rc.Close()
			return err
		}

		_, err = io.Copy(w, rc)
		rc.Close()
		if err != nil {
			return err
		}
	}
	return nil
}

func addStringToZip(zw *zip.Writer, name, content string) error {
	w, err := zw.Create(name)
	if err != nil {
		return err
	}
	_, err = io.WriteString(w, content)
	return err
}

func addFileToZip(zw *zip.Writer, srcPath, destName string) error {
	file, err := os.Open(srcPath)
	if err != nil {
		return err
	}
	defer file.Close()

	w, err := zw.Create(destName)
	if err != nil {
		return err
	}

	_, err = io.Copy(w, file)
	return err
}
