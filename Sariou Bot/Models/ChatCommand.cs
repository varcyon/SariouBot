using System;

namespace Sariou_Bot.Models
{
    public enum Permission
        {
            Viewer,
            Subscriber,
            VIP,
            Moderator,
            Broadcaster
        }
        public enum Automated
        {
            No,
            Yes
        }

    public class SimpleCommand : ViewModels.ViewModelBase
    {
        public int commandID { get; private set; }
        public string command { get; private set; }
        public string content { get; private set; }
        public Permission permission { get; private set; }
        public Automated auto { get; private set; }
        public float cooldown { get; private set; }
        public float timer { get; private set; }
        public Boolean onCooldown { get; private set; }

        public void SetOnCooldown(Boolean value) { this.onCooldown = value; }
        public void UpdateTimer(float value) { this.timer += value; }
        public void ResetTimer() { this.timer = 0; }
        public string createdOn { get; set; }
        public SimpleCommand()
        {
            commandID = 0;
            command = "command";
            content = "content";
            auto = Automated.No;
            cooldown = 0;
            permission = Permission.Viewer;
            onCooldown = false;
            timer = 0;
            createdOn = "1/1/1999";
        }
        public SimpleCommand(int commandID, string command, Automated automated, float cooldown, string content, Permission permission, string createdOn)
        {
            this.commandID = commandID;
            this.command = command;
            this.content = content;
            this.auto = automated;
            this.cooldown = cooldown;
            this.permission = permission;
            this.onCooldown = false;
            this.timer = 0;
            this.createdOn = createdOn;
        }
        public SimpleCommand(string command, int automated, float cooldown, string content, int permission)
        {
            this.command = command;
            this.content = content;
            this.auto = (Automated)automated;
            this.cooldown = cooldown;
            this.permission = (Permission)permission;
            this.onCooldown = false;
            this.timer = 0;
            this.createdOn = DateTime.Now.ToString();
        }
    }

    public class SoundCommand{
        private int commandID;
        private string command;
        private string filePath;
        private Permission permission;
        private float cooldown;


        public float timer { get; private set; }
        public string createdOn { get; set; }

        public Boolean onCooldown { get; private set; }
        public int CommandID { get => commandID; set => commandID = value; }
        public string Command { get => command; set => command = value; }
        public string FilePath { get => filePath; set => filePath = value; }
        public Permission Permission { get => permission; set => permission = value; }
        public float Cooldown { get => cooldown; set => cooldown = value; }
        public void SetOnCooldown(Boolean value) { this.onCooldown = value; }
        public void SetTimer(float value) { this.timer += value; }
        public void ResetTimer() { this.timer = 0; }
        public void UpdateTimer(float value) { this.timer += value; }
        public SoundCommand()
        {
            commandID = 0;
            command = "command";
            filePath = "File Path";
            cooldown = 0;
            permission = Permission.Viewer;
            onCooldown = false;
            timer = 0;
            createdOn = "1/1/1999";

        }
        public SoundCommand(int commandID, string command, string filepath, Permission permission, float cooldown, float timer, string createdOn)
        {
            this.commandID = commandID;
            this.command = command;
            this.filePath = filepath;
            this.permission = permission;
            this.cooldown = cooldown;
            this.timer = timer;
            this.createdOn = createdOn;
        }

        public SoundCommand(string command, string filepath, int permission, float cooldown)
        {
            this.command = command;
            this.filePath = filepath;
            this.permission = (Permission)permission;
            this.cooldown = cooldown;

        }

    }
}
