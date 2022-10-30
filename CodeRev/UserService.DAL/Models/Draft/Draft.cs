namespace UserService.DAL.Models.Draft
{
    public class Draft
    {
        public int MaxPosition { get; set; }
        public TextWithPosition[] Texts { get; set; }
        public CheckboxWithPosition[] Checkboxes { get; set; }
    }
}