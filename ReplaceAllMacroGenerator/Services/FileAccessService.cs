using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using CsvHelper;
using ReplaceAllMacroGenerator.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

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
            FilePickerFileType fileTypes = new ($"{fileExtension.ToUpper()} Files ({fileExtension})")
            {
                Patterns = new[] { $"*{fileExtension}" },
                AppleUniformTypeIdentifiers = new[] { $"public{fileExtension}" },
                MimeTypes = new[] { $"{fileExtension}/*" }
            };

            FilePickerOpenOptions options = new()
            {
                Title = $"Choose {fileExtension} file.",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { fileTypes }
            };

            string fileName = string.Empty;
            try
            {
                if (currentWindow?.StorageProvider is { CanOpen: true } storageProvider)
                {
                    IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(options);
                    if (files.Count > 0 && files[0].CanBookmark)
                    {
                        fileName = await files[0].SaveBookmarkAsync() ?? string.Empty;
                    }
                }

                return fileName;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return fileName;
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
            FilePickerFileType fileType = new ($"{fileExtension.ToUpper()} Files ({fileExtension})")
            {
                Patterns = new[] { $"*{fileExtension}" },
                AppleUniformTypeIdentifiers = new[] { $"public{fileExtension}" },
                MimeTypes = new[] { $"{fileExtension}/*" }
            };

            FilePickerSaveOptions options = new()
            {
                Title = $"Create a new {fileExtension} file.",
                DefaultExtension = $".{fileExtension}",
                ShowOverwritePrompt = true,
                FileTypeChoices = new List<FilePickerFileType>() { fileType }
            };

            string fileName = string.Empty;

            try
            {
                if (_currentWindow?.StorageProvider is { CanOpen: true } storageProvider)
                {
                    IStorageFile? file = await storageProvider.SaveFilePickerAsync(options);
                    if (file?.CanBookmark == true)
                    {
                        fileName = await file.SaveBookmarkAsync() ?? string.Empty;
                    }
                }

                return fileName;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return fileName;
            }
        }

        /// <summary>
        /// Builds a collection of POInfo from a CSV file.
        /// </summary>
        /// <param name="fileName">CSV file to use.</param>
        /// <returns>Collection of POInfo or an empty collection if there is an error.</returns>
        public static async Task<IEnumerable<ReplacementInfo>> LoadCSVAsync(string fileName, IMessenger theMessenger)
        {
            try
            {
                List<ReplacementInfo> replacementInformation = [];
                using TextReader theReader = File.OpenText(fileName);
                using CsvReader thecReader = new (theReader, CultureInfo.InvariantCulture);

                await foreach (ReplacementInfo currentPOInfo in thecReader.GetRecordsAsync<ReplacementInfo>())
                {
                    replacementInformation.Add(new ReplacementInfo()
                    {
                        OldInfo = currentPOInfo.OldInfo,
                        NewInfo = currentPOInfo.NewInfo
                    });
                }

                theReader.Close();
                return replacementInformation;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return [];
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
                await using StreamWriter streamWriter = File.CreateText(fileName);
                foreach (string item in information)
                {
                    await streamWriter.WriteLineAsync(item);
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
            }
        }
    }
}
