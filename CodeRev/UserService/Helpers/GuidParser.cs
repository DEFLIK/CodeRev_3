using System;

namespace UserService.Helpers
{
    public static class GuidParser
    {
        public static Tuple<Guid, string> TryParse(string id, string nameOfId)
        {
            var guid = new Guid();
            string errorString = null;
            try
            {
                guid = Guid.Parse(id);
            }
            catch (ArgumentNullException)
            {
                errorString = $"{nameOfId} to be parsed is null";
            }
            catch (FormatException)
            {
                errorString = $"{nameOfId} should be in UUID format";
            }
            return new Tuple<Guid, string>(guid, errorString);
        }
    }
}