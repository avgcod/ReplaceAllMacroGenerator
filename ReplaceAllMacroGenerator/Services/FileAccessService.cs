using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DynamicData;
using ReplaceAllMacroGenerator.Helpers;

namespace ReplaceAllMacroGenerator.Services
{
    public class FileAccessService : IFileAccessProvider
    {
        public IEnumerable<POInfo> LoadCSV(string fileName)
        {
            List<string> columns = new List<string>();
            using (CsvFileReader theReader = new CsvFileReader(File.OpenRead(fileName)))
            {
                while (theReader.ReadRow(columns))
                {
                    yield return new POInfo() { OldPO = columns[0], NewPO = columns[1] };
                }
            }
        }
        public async Task<IEnumerable<POInfo>> LoadCSVAsync(string fileName)
        {
            List<string> columns = new List<string>();
            List<POInfo> poInformation = new List<POInfo>();
            using (CsvFileReader theReader = new CsvFileReader(File.OpenRead(fileName)))
            {
                while (await theReader.ReadRowAsync(columns))
                {
                    poInformation.Add(new POInfo() { OldPO = columns[0], NewPO = columns[1] });
                }
            }

            return poInformation;
        }

        public bool SaveMacroFile(IEnumerable<string> information, string fileName)
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

        public async Task<bool> SaveMacroFileAsync(IEnumerable<string> information, string fileName)
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
