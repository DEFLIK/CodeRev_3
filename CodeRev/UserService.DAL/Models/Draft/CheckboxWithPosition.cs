namespace UserService.DAL.Models.Draft
{
    public class CheckboxWithPosition
    {
        public int Position { get; set; }
        public CheckboxRow[] Rows { get; set; }
    }
}