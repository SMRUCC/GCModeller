namespace DOM.Events {

    var started: boolean = false;
    var customEvents: {
        hasUpdate: Delegate.Func<boolean>,
        invoke: Delegate.Action,
        name?: string
    }[] = [];

    /**
     * Add custom user event.
     * 
     * @param trigger This lambda function detects that custom event is triggered or not.
     * @param handler This lambda function contains the processor code of your custom event.
    */
    export function Add(trigger: Delegate.Func<boolean> | StatusChanged, handler: Delegate.Action, tag: string = null) {
        if (trigger instanceof StatusChanged) {
            let predicate: StatusChanged = <StatusChanged>trigger;

            trigger = function () {
                return predicate.changed;
            }
        }

        customEvents.push({
            hasUpdate: trigger,
            invoke: handler,
            name: tag
        });

        if (!started) {
            setInterval(backgroundInternal, 10);
            started = true;
            TypeScript.logging.log("Start background worker...", TypeScript.ConsoleColors.DarkBlue);
        }
    }

    function backgroundInternal() {
        for (let hook of customEvents) {
            if (hook.hasUpdate()) {
                hook.invoke();
            }
        }
    }
}