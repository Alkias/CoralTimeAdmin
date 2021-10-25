using System.Collections.Generic;
using System.Web.Mvc;
using CoralTimeAdmin.Enums;
using CoralTimeAdmin.Helpers;

namespace CoralTimeAdmin.Controllers
{
    public abstract class BaseController : Controller
    {
        #region Noties

        /// <summary>
        /// Returns the Noties that had beed added to the session.
        /// </summary>
        protected List<Noty> GetNoties()
        {
            if (TempData["Noties"].IsNotNull())
            {
                return (TempData["Noties"] as string).FromJson<List<Noty>>();
            }

            return new List<Noty>();
        }

        /// <summary>
        /// Adds a list of noties to the session.
        /// </summary>
        /// <param name="noties">The noties that will be added to the session.</param>
        protected void AddNoties(List<Noty> noties)
        {
            if (noties.IsEmpty())
            {
                return;
            }

            var json = noties.ToJson();
            TempData["Noties"] = json;
        }

        /// <summary>
        /// Adds a noty to the session.
        /// </summary>
        /// <param name="noty">The noty that will be added to the session.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        protected void AddNoty(
            Noty noty,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight
        )
        {
            var noties = GetNoties();
            noties.Add(noty);
            AddNoties(noties);
        }

        /// <summary>
        /// Creates an Alert noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        protected void Alert(
            string text,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight,
            NotyCloseWith closeWith = NotyCloseWith.both
        )
        {
            var noty = Noty.Alert(text, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            AddNoty(noty);
        }

        /// <summary>
        /// Creates an Success noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        protected void Success(
            string text,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight,
            NotyCloseWith closeWith = NotyCloseWith.both
        )
        {
            var noty = Noty.Success(text, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            AddNoty(noty);
        }

        /// <summary>
        /// Creates an Error noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        /// <param name="closeWith">The close with.</param>
        protected void Error(
            string text,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight,
            NotyCloseWith closeWith = NotyCloseWith.both
        )
        {
            var noty = Noty.Error(text, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            AddNoty(noty);
        }

        /// <summary>
        /// Creates an Info noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        protected void Info(
            string text,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight,
            NotyCloseWith closeWith = NotyCloseWith.both
        )
        {
            var noty = Noty.Info(text, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            AddNoty(noty);
        }

        /// <summary>
        /// Creates an Warning noty.
        /// </summary>
        /// <param name="text">The Noty content. Can contain HTML.</param>
        /// <param name="layout">
        /// The layout of the Noty. Values: top, topLeft, topCenter, topRight, center, centerLeft,
        /// centerRight, bottom, bottomLeft, bottomCenter, bottomRight.
        /// </param>
        /// <param name="timeout">Delay for closing event in milliseconds (ms). Set '0' for sticky notifications.</param>
        /// <param name="progressBar">Displays a progress bar if timeout is not 0.</param>
        /// <param name="animationOpen">The open animation.</param>
        /// <param name="animationClose">The close animation.</param>
        protected void Warning(
            string text,
            NotyLayout layout = NotyLayout.topRight,
            int timeout = 3000,
            bool progressBar = false,
            AnimationType animationOpen = AnimationType.bounceInRight,
            AnimationType animationClose = AnimationType.bounceOutRight,
            NotyCloseWith closeWith = NotyCloseWith.both
        )
        {
            var noty = Noty.Warning(text, layout, timeout, progressBar, animationOpen, animationClose, closeWith);
            AddNoty(noty, layout, timeout, progressBar, animationOpen, animationClose);
        }

        #endregion Noties
    }
}