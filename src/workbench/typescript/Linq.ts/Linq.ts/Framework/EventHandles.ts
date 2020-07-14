namespace Internal {

    export module EventHandles {

        /**
         * find all elements' id on current html page
        */
        export function findAllElement(attr: "id" | "name" = "id"): string[] {
            let elements = document.querySelectorAll(`*[${attr}]`);
            let id: string[] = [];

            for (let node of $ts(elements).ToArray(false)) {
                id.push(node.id);
            }

            return id;
        }

        export function hookEventHandles(app: {}) {
            let elements: string[] = findAllElement();
            let type = TypeScript.Reflection.$typeof(app);

            // handle clicks
            for (let methodName of type.methods) {
                if (elements.indexOf(methodName) > -1) {
                    let arguments = parseFunctionArgumentNames(app[methodName]);

                    if (arguments.length == 0 || arguments.length == 2) {
                        $ts(`#${methodName}`).onclick = <any>function (handler, evt): any {
                            return app[methodName](handler, evt);
                        }

                        console.info(`%c[${type.class}] hook onclick for #${methodName}...`, "color:green;");
                    }
                }
            }

            elements = findAllElement("name");

            for (let methodName of type.methods) {
                if (elements.indexOf(methodName) > -1) {
                    let onchange = app[methodName];
                    let arguments = parseFunctionArgumentNames(onchange);

                    if (arguments.length == 1 && arguments[0] == "value") {
                        document.getElementsByName(methodName).forEach(function (a) {
                            let tag = a.tagName.toLowerCase();

                            if (tag == "input" || tag == "textarea") {
                                let type = a.getAttribute("type");

                                if (!isNullOrUndefined(type) && type.toLowerCase() == "file") {
                                    a.onchange = function () {
                                        let value = $input(a).files;
                                        return app[methodName](value);
                                    }
                                } else {
                                    a.onchange = function () {
                                        let value = DOM.InputValueGetter.unifyGetValue(a);
                                        return app[methodName](value);
                                    }
                                }
                            } else if (tag == "select") {
                                a.onchange = function () {
                                    let value = DOM.InputValueGetter.unifyGetValue(a);
                                    return app[methodName](value);
                                }
                            } else {
                                TypeScript.logging.log(`invalid tag name: ${a.tagName}!`, "red");
                            }
                        });
                    }
                }
            }
        }

        const STRIP_COMMENTS = /((\/\/.*$)|(\/\*[\s\S]*?\*\/))/mg;
        const ARGUMENT_NAMES = /([^\s,]+)/g;

        export function parseFunctionArgumentNames(func: any): string[] {
            let fnStr = func.toString().replace(STRIP_COMMENTS, '');
            let result = fnStr.slice(fnStr.indexOf('(') + 1, fnStr.indexOf(')')).match(ARGUMENT_NAMES);

            if (result === null) {
                return [];
            } else {
                return result;
            }
        }
    }
}