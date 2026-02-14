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

func main() {
	fmt.Println("BepInEx Build Updater (Go version)")
	fmt.Println("==================================")

	fmt.Println("Fetching builds from BepInEx build server...")
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

	fmt.Println("\nAvailable Builds (matched with GitHub commits):")

	type DisplayItem struct {
		BuildID string
		Hash    string
		Message string
	}
	var displayList []DisplayItem

	// Sort builds by ID descending
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

	fmt.Print("\nEnter build number (e.g. 752) or index: ")
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
		fmt.Printf("Fetching hash for build #%s...\n", buildNumber)
		commitHash, _ = getHashForBuild(buildNumber)
	}

	if buildNumber == "" || commitHash == "" {
		fmt.Println("Invalid input or could not find build hash.")
		return
	}

	fmt.Println("\nSelect Architecture:")
	fmt.Println("[0] x86 (Steam / Itch.io)")
	fmt.Println("[1] x64 (Epic Games / MS Store)")
	fmt.Print("Enter index [default: 0]: ")
	archInput, _ := reader.ReadString('\n')
	archInput = strings.TrimSpace(archInput)

	arch := "x86"
	if archInput == "1" {
		arch = "x64"
	}

	url := fmt.Sprintf("https://builds.bepinex.dev/projects/bepinex_be/%s/BepInEx-Unity.IL2CPP-win-%s-6.0.0-be.%s%%2B%s.zip", buildNumber, arch, buildNumber, commitHash)
	fmt.Printf("\nDownloading (%s): %s\n", arch, url)

	zipPath := filepath.Join(os.TempDir(), fmt.Sprintf("BepInEx_%s_%s.zip", buildNumber, arch))
	if err := downloadFile(url, zipPath); err != nil {
		fmt.Printf("Download failed: %v\n", err)
		return
	}
	defer os.Remove(zipPath)

	fmt.Println("Extracting...")
	exePath, _ := os.Executable()
	amongUsDir := filepath.Dir(exePath)

	if err := extractZip(zipPath, amongUsDir); err != nil {
		fmt.Printf("Extraction failed: %v\n", err)
		return
	}

	fmt.Println("\nUpdate completed successfully!")
	fmt.Println("\nPress Enter to exit...")
	bufio.NewReader(os.Stdin).ReadBytes('\n')
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
	req.Header.Set("User-Agent", "BepInExUpdater")

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

func extractZip(zipPath, targetDir string) error {
	r, err := zip.OpenReader(zipPath)
	if err != nil {
		return err
	}
	defer r.Close()

	for _, f := range r.File {
		fpath := filepath.Join(targetDir, f.Name)

		if f.FileInfo().IsDir() {
			os.MkdirAll(fpath, os.ModePerm)
			continue
		}

		if err := os.MkdirAll(filepath.Dir(fpath), os.ModePerm); err != nil {
			return err
		}

		outFile, err := os.OpenFile(fpath, os.O_WRONLY|os.O_CREATE|os.O_TRUNC, f.Mode())
		if err != nil {
			return err
		}

		rc, err := f.Open()
		if err != nil {
			outFile.Close()
			return err
		}

		_, err = io.Copy(outFile, rc)
		outFile.Close()
		rc.Close()

		if err != nil {
			return err
		}
	}
	return nil
}
