using System;

namespace OpenGLApp.src.DataClasses
{

    public abstract class User
    {
        protected string name;
        protected string id;
        protected string address;
        protected string city;
        protected string state;
        protected string zip;

        public User(params string[] args)
        {
            Build(args);
        }

        protected void Build(params string[] args)
        {
            if (args.Length < 6)
                return;
        }
    }

}
