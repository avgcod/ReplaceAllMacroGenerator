using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using ReplaceAllMacroGenerator.Models;
using CsvHelper;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Globalization;
using CommunityToolkit.Mvvm.Messaging;
using System.Linq;

namespace ReplaceAllMacroGenerator.Services
{
    /// <summary>
    /// Provides IO operation methods.
    /// </summary>
    public static class FileAccessService
    {
        /// <summary>
        /// Opens a file chooser dialog and returns the chosen file.
        /// </summary>
        /// <param name="currentWindow">Parent window for the dialog.</param>
        /// <param name="fileExtension">File extension to filter by.</param>
        /// <returns>The chosen file or an empty string if none selected or there is an error.</returns>
        public static async Task<string> ChooseOpenFileAsync(Window currentWindow, string fileExtension, IMessenger theMessenger)
        {
            FilePickerFileType fileTypes = new FilePickerFileType($"{fileExtension.ToUpper()} Files ({fileExtension})")
            {
                Patterns = new[] { $"*{fileExtension}" },
                AppleUniformTypeIdentifiers = new[] { $"public{fileExtension}" },
                MimeTypes = new[] { $"{fileExtension}/*" }
            };

            FilePickerOpenOptions options = new FilePickerOpenOptions()
            {
                Title = $"Choose {fileExtension} file.",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { fileTypes }
            };

            try
            {
                IReadOnlyList<IStorageFile>? files = await currentWindow?.StorageProvider.OpenFilePickerAsync(options);
                if (files != null && files.Any() && files[0].CanBookmark)
                {
                    return await files[0].SaveBookmarkAsync() ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return string.Empty;
            }
        }

        /// <summary>
        /// Opens a save file dialog and returns the chosen file.
        /// </summary>
        /// <param name="_currentWindow">Parent window for the dialog.</param>
        /// <param name="fileExtension">File extension to filter by.</param>
        /// <returns>The chosen file or an empty string if none selected or there is an error.</returns>
        public static async Task<string> ChooseSaveFileAsync(Window _currentWindow, string fileExtension, IMessenger theMessenger)
        {
            FilePickerFileType fileType = new FilePickerFileType($"{fileExtension.ToUpper()} Files ({fileExtension})")
            {
                Patterns = new[] { $"*{fileExtension}" },
                AppleUniformTypeIdentifiers = new[] { $"public{fileExtension}" },
                MimeTypes = new[] { $"{fileExtension}/*" }
            };

            FilePickerSaveOptions options = new FilePickerSaveOptions()
            {
                Title = $"Create a new {fileExtension} file.",
                DefaultExtension = $".{fileExtension}",
                ShowOverwritePrompt = true,
                FileTypeChoices = new List<FilePickerFileType>() { fileType }
            };

            try
            {
                IStorageFile? file = await _currentWindow?.StorageProvider.SaveFilePickerAsync(options);
                if (file != null && file.CanBookmark)
                {
                    return await file.SaveBookmarkAsync() ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return string.Empty;
            }
        }

        /// <summary>
        /// Builds a collection of POInfo from a CSV file.
        /// </summary>
        /// <param name="fileName">CSV file to use.</param>
        /// <returns>Collection of POInfo or an empty collection if there is an error.</returns>
        public static IEnumerable<POInfo> LoadCSV(string fileName, IMessenger theMessenger)
        {
            try
            {
                List<POInfo> poInformation = new List<POInfo>();
                using TextReader theReader = File.OpenText(fileName);
                using CsvReader thecReader = new CsvReader(theReader, CultureInfo.InvariantCulture);

                IEnumerable<POInfo> loadedPOInfo = thecReader.GetRecords<POInfo>();
                foreach (POInfo currentPOInfo in loadedPOInfo)
                {
                    poInformation.Add(new POInfo()
                    {
                        OldPO = currentPOInfo.OldPO,
                        NewPO = currentPOInfo.NewPO
                    });
                }

                theReader.Close();
                return poInformation;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return new List<POInfo>();
            }            

        }

        /// <summary>
        /// Builds a collection of POInfo from a CSV file.
        /// </summary>
        /// <param name="fileName">CSV file to use.</param>
        /// <returns>Collection of POInfo or an empty collection if there is an error.</returns>
        public static async Task<IEnumerable<POInfo>> LoadCSVAsync(string fileName, IMessenger theMessenger)
        {
            try
            {
                List<POInfo> poInformation = new List<POInfo>();
                using TextReader theReader = File.OpenText(fileName);
                using CsvReader thecReader = new CsvReader(theReader, CultureInfo.InvariantCulture);

                IAsyncEnumerable<POInfo> loadedPOInfo = thecReader.GetRecordsAsync<POInfo>();
                await foreach (POInfo currentPOInfo in loadedPOInfo)
                {
                    poInformation.Add(new POInfo()
                    {
                        OldPO = currentPOInfo.OldPO,
                        NewPO = currentPOInfo.NewPO
                    });
                }

                theReader.Close();
                return poInformation;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return new List<POInfo>();
            }            
        }

        /// <summary>
        /// Saves a macro file.
        /// </summary>
        /// <param name="information">Collection of strings representing the macro.</param>
        /// <param name="fileName">Macro file name.</param>
        public static void SaveMacroFile(IEnumerable<string> information, string fileName, IMessenger theMessenger)
        {
            try
            {
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    foreach (string item in information)
                    {
                        streamWriter.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
            }
        }

        /// <summary>
        /// Saves a macro file.
        /// </summary>
        /// <param name="information">Collection of strings representing the macro.</param>
        /// <param name="fileName">Macro file name.</param>
        public static async Task SaveMacroFileAsync(IEnumerable<string> information, string fileName, IMessenger theMessenger)
        {
            try
            {
                using (StreamWriter streamWriter = File.CreateText(fileName))
                {
                    foreach (string item in information)
                    {
                        await streamWriter.WriteLineAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
            }
        }
    }
}
