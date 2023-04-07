using System;

namespace AppNov14.Models.Comment
{
    public class CommentModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Employee { get; set; }

        public string Answer { get; set; }

        public DateTime InsertDate { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }
    }
}