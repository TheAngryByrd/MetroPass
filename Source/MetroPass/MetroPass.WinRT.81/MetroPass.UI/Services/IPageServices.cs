using System;
using System.Threading.Tasks;

namespace MetroPass.UI.Services
{
    public interface IPageServices
    {
        Task Show(string title, string message);

        Task Show(string message);

        void Toast(string message);

        Task<bool> EnsureUnsnapped();
    }
}