namespace UserService.DAL.Models.Draft
{
    public class Draft
    {
        public int MaxPosition { get; set; }
        public ValueWithPosition[] Texts { get; set; }
        public Checkbox[] Checkboxes { get; set; }
    }
}