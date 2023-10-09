using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using ReplaceAllMacroGenerator.Models;
using CsvHelper;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Globalization;

namespace ReplaceAllMacroGenerator.Services
{
    public static class FileAccessService
    {
        public static async Task<IStorageFile?> ChooseCSVFileAsync(Window _currentWindow)
        {
            FilePickerFileType fileTypes = new FilePickerFileType("CSV Files (.csv)")
            {
                Patterns = new[] { "*.csv" },
                AppleUniformTypeIdentifiers = new[] { "public.csv" },
                MimeTypes = new[] { "csv/*" }
            };

            FilePickerOpenOptions options = new FilePickerOpenOptions()
            {
                Title = "Choose csv file.",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { fileTypes }
            };

            IReadOnlyList<IStorageFile>? files = await _currentWindow?.StorageProvider.OpenFilePickerAsync(options);

            return files.Count >= 1 ? files[0] : null;
        }

        public static async Task<IStorageFile?> ChooseBASFileAsync(Window _currentWindow)
        {
            FilePickerFileType fileType = new FilePickerFileType("BAS Files (.bas)")
            {
                Patterns = new[] { "*.bas" },
                AppleUniformTypeIdentifiers = new[] { "public.bas" },
                MimeTypes = new[] { "bas/*" }
            };

            FilePickerSaveOptions options = new FilePickerSaveOptions()
            {
                Title = "Create a new bas file.",
                DefaultExtension = ".bas",
                ShowOverwritePrompt = true,
                FileTypeChoices = new List<FilePickerFileType>() { fileType }
            };

            IStorageFile? file = await _currentWindow?.StorageProvider.SaveFilePickerAsync(options);

            return file;
        }

        public static List<POInfo> LoadCSV(string fileName)
        {
            List<POInfo> poInformation = new List<POInfo>();
            using StreamReader thesReader = new StreamReader(fileName);
            using CsvReader thecReader = new CsvReader(thesReader, CultureInfo.InvariantCulture);

            IEnumerable<POInfo> loadedPOInfo = thecReader.GetRecords<POInfo>();
            foreach (POInfo currentPOInfo in loadedPOInfo)
            {
                poInformation.Add(new POInfo()
                {
                    OldPO = currentPOInfo.OldPO,
                    NewPO = currentPOInfo.NewPO
                });
            }

            return poInformation;

        }
        public static async Task<List<POInfo>> LoadCSVAsync(string fileName)
        {
            List<POInfo> poInformation = new List<POInfo>();
            using StreamReader thesReader = new StreamReader(fileName);
            using CsvReader thecReader = new CsvReader(thesReader, CultureInfo.InvariantCulture);

            IAsyncEnumerable<POInfo> loadedPOInfo = thecReader.GetRecordsAsync<POInfo>();
            await foreach (POInfo currentPOInfo in loadedPOInfo)
            {
                poInformation.Add(new POInfo()
                {
                    OldPO = currentPOInfo.OldPO,
                    NewPO = currentPOInfo.NewPO
                });
            }

            return poInformation;
        }

        public static bool SaveMacroFile(IEnumerable<string> information, string fileName)
        {
            try
            {
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    foreach (string item in information)
                    {
                        streamWriter.WriteLine(item);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> SaveMacroFileAsync(IEnumerable<string> information, string fileName)
        {
            try
            {
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    foreach (string item in information)
                    {
                        await streamWriter.WriteLineAsync(item);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
