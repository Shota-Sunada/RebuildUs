package main

import (
"encoding/json"
"fmt"
"os"
)

func main() {
files := []string{
"RebuildUs/Localization/Translations/English.json",
"RebuildUs/Localization/Translations/Japanese.json",
}

for _, file := range files {
err := formatJSON(file)
if err != nil {
fmt.Printf("Error formatting %s: %v\n", file, err)
} else {
fmt.Printf("Successfully formatted %s\n", file)
}
}
}

func formatJSON(path string) error {
data, err := os.ReadFile(path)
if err != nil {
return err
}

// Parse JSON
var parsed map[string]interface{}
if err := json.Unmarshal(data, &parsed); err != nil {
return err
}

// Marshal back to JSON with 2-space indentation. 
// Go's json.MarshalIndent automatically sorts map keys in alphabetical order.
formatted, err := json.MarshalIndent(parsed, "", "  ")
if err != nil {
return err
}

// Add a trailing newline
formatted = append(formatted, '\n')

// Save the formatted JSON back to the file
return os.WriteFile(path, formatted, 0644)
}
