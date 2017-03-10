using System;
using System.Collections.Generic;
using System.Linq;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Run
{
    public static class LifeCycleManager
    {
        public static State GetState(IEnumerable<IComponent> components)
        {
            var state = State.Undefined; // Fake state
            foreach (var component in components)
            {
                if (state == State.Undefined || component.State < state)
                    state = component.State;
            }
            return state;
        }

        public static void Link(DynamicMap context, IEnumerable<IComponent> components)
        {
            Link(context, new ComponentSet(components));
        }

        public static void Link(DynamicMap context, ComponentSet components)
        {
            var orderedList = components.GetAllOrdered();
            foreach (var component in orderedList)
                component.Link(context, components);
        }

        public static void LinkAndOpen(DynamicMap context, IEnumerable<IComponent> components)
        {
            LinkAndOpen(context, new ComponentSet(components));
        }

        public static void LinkAndOpen(DynamicMap context, ComponentSet components)
        {
            Link(context, components);
            Open(components);
        }

        public static void Open(IEnumerable<IComponent> components)
        {
            IList<IComponent> opened = new List<IComponent>();

            var enumerable = components as IComponent[] ?? components.ToArray();

            try
            {
                foreach (var component in enumerable)
                {
                    if (component.State != State.Opening && component.State != State.Ready)
                        component.Open();
                    opened.Add(component);
                }
            }
            catch (Exception ex)
            {
                LogWriter.Trace(enumerable, "Microservice opening failed with error " + ex);
                ForceClose(opened, false);
                throw;
            }
        }

        public static void Open(ComponentSet components)
        {
            Open(components.GetAllOrdered());
        }

        public static void Close(IEnumerable<IComponent> components)
        {
            // Close in reversed order
            IList<IComponent> toClose = new List<IComponent>();
            var enumerable = components as IComponent[] ?? components.ToArray();

            foreach (var component in enumerable)
            {
                toClose.Insert(0, component);
            }

            try
            {
                foreach (var component in toClose)
                {
                    if (component.State == State.Ready)
                        component.Close();
                }
            }
            catch (Exception ex)
            {
                LogWriter.Trace(enumerable, "Microservice closure failed with error " + ex);
                throw;
            }
        }

        public static void Close(ComponentSet components)
        {
            Close(components.GetAllOrdered());
        }

        public static void ForceClose(IEnumerable<IComponent> components)
        {
            ForceClose(components, true);
        }

        public static void ForceClose(IEnumerable<IComponent> components, bool throwException)
        {
            // Close in reversed order
            IList<IComponent> toClose = new List<IComponent>();
            var enumerable = components as IComponent[] ?? components.ToArray();

            foreach (var component in enumerable)
            {
                toClose.Insert(0, component);
            }

            MicroserviceError firstError = null;

            foreach (var component in toClose)
            {
                try
                {
                    if (component.State == State.Ready)
                        component.Close();
                }
                catch (MicroserviceError ex)
                {
                    LogWriter.Trace(enumerable, "Microservice closure failed with error " + ex);
                    firstError = firstError ?? ex;
                }
                catch (Exception ex)
                {
                    LogWriter.Trace(enumerable, "Microservice closure failed with error " + ex);
                    firstError = firstError ?? new UnknownError(
                        "CloseFailed",
                        "Failed to close component " + component + ": " + ex
                        ).Wrap(ex);
                }
            }

            if (firstError != null && throwException)
                throw firstError;
        }

        public static void ForceClose(ComponentSet components)
        {
            ForceClose(components.GetAllOrdered(), true);
        }

        public static void ForceClose(ComponentSet components, bool throwException)
        {
            ForceClose(components.GetAllOrdered(), throwException);
        }
    }
}