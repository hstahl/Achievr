using System;
using System.Reflection;

namespace Achievr.Model
{
    public class Achievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Unlocked { get; set; }
        public int ScoreValue { get; set; }

        public Achievement() { }

        public Achievement(string title, string description, int scoreValue = 10)
        {
            this.Title = title;
            this.Description = description;
            this.ScoreValue = scoreValue;
            this.Unlocked = false;
        }

        public void ToggleUnlocked()
        {
            Unlocked = !Unlocked;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(this.GetType()))
            {
                Achievement other = (Achievement)obj;
                return this.Title.Equals(other.Title) &&
                       this.Description.Equals(other.Description) &&
                       this.ScoreValue.Equals(other.ScoreValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode() +
                   Description.GetHashCode() +
                   ScoreValue;
        }

        public override string ToString()
        {
            return Title + " (" + ScoreValue + " p)";
        }
    }

    public class DueAchievement : Achievement
    {
        public DateTime DueTime { get; }

        public DueAchievement(string title, string description, int score_value, DateTime dueTime)
            : base(title, description, score_value)
        {
            this.DueTime = dueTime;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(this.GetType()))
            {
                DueAchievement other = (DueAchievement)obj;
                return this.DueTime.Equals(other.DueTime) &&
                       base.Equals(obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return DueTime.GetHashCode() +
                   base.GetHashCode();
        }
    }
}