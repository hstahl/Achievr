using System;
using System.Reflection;

namespace Achievr
{
    class Achievement
    {
        string title;
        string description;
        bool unlocked;
        int score_value;

        public Achievement(string title, string description, int score_value = 10)
        {
            this.title = title;
            this.description = description;
            this.score_value = score_value;
            unlocked = false;
        }

        public bool IsUnlocked()
        {
            return unlocked;
        }

        public void ToggleUnlocked()
        {
            unlocked = !unlocked;
        }

        public int GetScoreValue()
        {
            return score_value;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(this.GetType()))
            {
                Achievement other = (Achievement)obj;
                return this.title.Equals(other.title) &&
                       this.description.Equals(other.description) &&
                       this.score_value.Equals(other.score_value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return title.GetHashCode() +
                   description.GetHashCode() +
                   score_value;
        }

        public override string ToString()
        {
            return title + " (" + score_value + " p)";
        }
    }

    class DueAchievement : Achievement
    {
        DateTime due_time;
        public DueAchievement(string title, string description, int score_value, DateTime due_time)
            : base(title, description, score_value)
        {
            this.due_time = due_time;
        }

        public DateTime GetDueDate()
        {
            return due_time;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().IsAssignableFrom(this.GetType()))
            {
                DueAchievement other = (DueAchievement)obj;
                return this.due_time.Equals(other.due_time) &&
                       base.Equals(obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return due_time.GetHashCode() +
                   base.GetHashCode();
        }
    }
}