using System;
using OttaMatta.Data.Models;
namespace OttaMatta.Data.Access
{
    public interface IWebObjectStorage
    {
        WebObject GetUrlObject(string url);
        void SetUrlObject(WebObject urlObject);
    }
}
