package main

import (
"bufio"
"encoding/csv"
"fmt"
"os"
"os/exec"
"strings"
)

func main() {
reader := bufio.NewReader(os.Stdin)

for {
fmt.Print("Enter Translation Key (leave blank to finish): ")
key, _ := reader.ReadString('\n')
key = strings.TrimSpace(key)

// Finish if key is empty
if key == "" {
break
}

fmt.Print("Enter English Text: ")
enText, _ := reader.ReadString('\n')
enText = strings.TrimSpace(enText)

csvPath := "Translations.csv"

// Open file in append mode
f, err := os.OpenFile(csvPath, os.O_APPEND|os.O_WRONLY, 0644)
if err != nil {
fmt.Printf("Error opening CSV: %v\n", err)
continue
}

writer := csv.NewWriter(f)

// Escape backslashes if any
err = writer.Write([]string{key, enText, ""}) // Japanese is empty by default
if err != nil {
fmt.Printf("Error writing to CSV: %v\n", err)
f.Close()
continue
}

writer.Flush()
f.Close()
fmt.Printf("Added new translation: [%s] -> \"%s\"\n\n", key, enText)
}

fmt.Println("Running TranslateSync to update JSON and Enum...")

cmd := exec.Command("go", "run", "Tools/TranslateSync/translate_sync.go")
cmd.Stdout = os.Stdout
cmd.Stderr = os.Stderr
if err := cmd.Run(); err != nil {
fmt.Printf("Error running translate_sync.go: %v\n", err)
} else {
fmt.Println("Done. Please check the updated files.")
}
}
