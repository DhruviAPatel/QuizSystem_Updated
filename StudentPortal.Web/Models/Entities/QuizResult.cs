using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentPortal.Web.Models.Entities
{
    public class QuizResult
    {
        [Key]
        public int ResultId { get; set; }

        [ForeignKey("Quiz")]
        public int QuizId { get; set; }

        public int UserId { get; set; }

        public int Score { get; set; }

        public DateTime TakenDate { get; set; }

        public virtual Quiz Quiz { get; set; }
    }
}

