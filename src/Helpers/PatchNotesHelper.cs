using System.Text;

namespace Cs2Bot.Helpers
{
    internal class PatchNotesHelper
    {
        public PatchNotesHelper()
        {

        }
        
        // Strips out formatting returned from API into Discord friendly formatting.
        public string ParseContent(string content)
        {
            var builder = new StringBuilder(content);
            // Replace [*] with '*' for discord list format
            builder.Replace("[*]", "*");

            // Remove [list] descriptors
            builder.Replace("[list]", "");
            builder.Replace("[/list]", "");

            // Remove heading brackets and replace with bold formatting
            builder.Replace("[ ", "**");
            builder.Replace(" ]\n", "**\n");
            return builder.ToString();
        }
    }
}
