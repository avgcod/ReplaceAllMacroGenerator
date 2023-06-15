using ReplaceAllMacroGenerator.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReplaceAllMacroGenerator.Services
{
    public interface IFileAccessProvider
    {
        IEnumerable<POInfo> LoadCSV(string fileName);
        Task<IEnumerable<POInfo>> LoadCSVAsync(string fileName);
        bool SaveMacroFile(IEnumerable<string> information, string fileName);
        Task<bool> SaveMacroFileAsync(IEnumerable<string> information, string fileName);
    }
}
