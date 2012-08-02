using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using DotLiquid.Exceptions;
using DotLiquid.FileSystems;

namespace DotLiquid.Modules.Tests.Util
{
    public class ResourceFileSystem : IFileSystem
    {
        public string Root { get; set; }
        public Assembly Assembly { get; set; }

        public ResourceFileSystem(string root, Assembly assembly)
		{
			Root = root;
            Assembly = assembly;
		}

        public string ReadTemplateFile(Context context, string templateName)
        {
            string templatePath = (string)context[templateName];
            string fullPath = FullPath(templatePath).Replace('/', '.').Replace('\\', '.');

            Stream s = Assembly.GetManifestResourceStream(fullPath);

            if (s == null)
                throw new FileSystemException("File resource doesn't exist", templatePath);

            TextReader tr = new StreamReader(s);

            return tr.ReadToEnd();
        }

        public string FullPath(string templatePath)
        {
            if (templatePath == null || !Regex.IsMatch(templatePath, @"^[^.\/][a-zA-Z0-9_\/]+$"))
                throw new FileSystemException("Illegal template name", templatePath);

            string templateDir = Path.GetDirectoryName(templatePath);

            string fullPath = templatePath.Contains("/")
                ? Path.Combine(Path.Combine(Root, templateDir ?? ""), string.Format("_{0}.liquid", Path.GetFileName(templatePath)))
                : Path.Combine(Root, string.Format("_{0}.liquid", templatePath));

            return fullPath;
        }
    }
}
