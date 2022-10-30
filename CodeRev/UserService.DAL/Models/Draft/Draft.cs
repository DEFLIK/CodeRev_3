namespace UserService.DAL.Models.Draft
{
    public class Draft
    {
        public int MaxPosition { get; set; }
        public TextGroup[] Texts { get; set; }
        public CheckboxGroup[] CheckboxGroups { get; set; }
    }
}