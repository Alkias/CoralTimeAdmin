using System.Collections.Generic;
using CoralTimeAdmin.Enums;

namespace CoralTimeAdmin.Helpers
{
    public class Noty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Noty"/> class.
        /// </summary>
        public Noty() { }

        /// <summary>
        /// Default Noty ctor.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="type">The type of the Noty. Values: alert, success, error, warning, info</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public Noty(string text,
            NotyType type,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            this.text = text;
            this.type = type.ToString();
            this.layout = layout.ToString();
            this.timeout = timeout;
            this.progressBar = progressBar;
            animation = new NotyAnimation(animationOpen, animationClose);

            this.closeWith = new List<string>();
            switch (closeWith)
            {
                case NotyCloseWith.both:
                    {
                        this.closeWith.Add("click");
                        this.closeWith.Add("button");
                        return;
                    }
                case NotyCloseWith.click:
                    {
                        this.closeWith.Add("click");
                        return;
                    }
                case NotyCloseWith.button:
                    {
                        this.closeWith.Add("button");
                        return;
                    }
            }
        }

        /// <summary>
        /// alert, success, error, warning, info.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </summary>
        public string layout { get; set; }

        /// <summary>
        /// This string can contain HTML too.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.
        /// </summary>
        public int timeout { get; set; }

        /// <summary>
        /// Displays a progress bar if timeout is not 0.
        /// </summary>
        public bool progressBar { get; set; }

        /// <summary>
        /// The open and close animations.
        /// </summary>
        public NotyAnimation animation { get; set; }

        /// <summary>
        /// click, button
        /// </summary>
        public List<string> closeWith { get; set; }

        /// <summary>
        /// Creates an Alert noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public static Noty Alert(string text,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            return new Noty(text, NotyType.alert, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
        }

        /// <summary>
        /// Creates an Info noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public static Noty Info(string text,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            return new Noty(text, NotyType.info, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
        }

        /// <summary>
        /// Creates an Error noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public static Noty Error(string text,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            return new Noty(text, NotyType.danger, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            //return new Noty(text, NotyType.error, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
        }

        /// <summary>
        /// Creates an Success noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public static Noty Success(string text,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            return new Noty(text, NotyType.success, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
        }

        /// <summary>
        /// Creates an Warning noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft, centerRight, bottom, bottomLeft, bottomCenter, bottomRight.</param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        public static Noty Warning(string text,
            NotyLayout layout,
            int timeout,
            bool progressBar,
            AnimationType animationOpen,
            AnimationType animationClose,
            NotyCloseWith closeWith)
        {
            return new Noty(text, NotyType.warning, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
        }
    }

    /// <summary>
    /// Holds the classes for the open and close animations of the Noty.
    /// </summary>
    public class NotyAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotyAnimation"/> class.
        /// </summary>
        public NotyAnimation()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotyAnimation"/> class.
        /// </summary>
        /// <param name="open">The open.</param>
        /// <param name="close">The close.</param>
        public NotyAnimation(
            AnimationType open,
            AnimationType close)
        {
            this.open = "animated {0}".FormatString(open.ToString());
            this.close = "animated {0}".FormatString(close.ToString());
        }

        /// <summary>
        /// The class name of the open animation.
        /// </summary>
        /// <value>
        /// The open.
        /// </value>
        public string open { get; set; }

        /// <summary>
        /// The class name of the close animation.
        /// </summary>
        /// <value>
        /// The close.
        /// </value>
        public string close { get; set; }
    }
}