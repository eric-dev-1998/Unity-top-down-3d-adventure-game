namespace Assets.Scripts.Quest_System
{
    public class QuestNotification
    {
        public enum NotificationType { Activate, Complete, Track, Update };
        public string message;
        public string questName;

        public QuestNotification(string questName, string message)
        { 
            this.questName = questName;
            this.message = message;
        }
    }
}
