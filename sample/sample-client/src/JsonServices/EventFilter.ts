export interface IEventFilter {
    [key: string]: string | null;
}

export class EventFilter {
    public static matches(eventFilter?: IEventFilter | null, eventArgs?: object | null): boolean {
        // empty filter matches anything
        if (eventFilter === null ||
            eventFilter === undefined ||
            Object.keys(eventFilter).length === 0 ||
			Object.keys(eventFilter).find(k => ("" + eventFilter[k]) !== "") === undefined) {
			return true;
		}

        // empty event arguments doesn't match any filter except for empty filter
        if (eventArgs === null || eventArgs === undefined) {
            return false;
        }

        // match individual properties based on their types
        for (const key in eventFilter) {
            // check eventFilter's own properties
            if (eventFilter.hasOwnProperty(key)) {
                const filterValue = eventFilter[key] || "";
                const propertyValue = eventArgs[key];
                if (!this.valueMatches(filterValue, propertyValue)) {
                    return false;
                }
            }
        }

        return true;
    }

    public static valueMatches(filterValue: string, propertyValue: any) {
        // property not found
        if (propertyValue === undefined) {
            return false;
        }

        // empty filter matches anything
        if ((filterValue || "") === "") {
            return true;
        }

        // match based on the property value type
        if (filterValue === (propertyValue || "").toString()) {
            return true;
        } else if (typeof propertyValue === "string") {
			return this.stringMatches(filterValue, propertyValue);
		} else if (typeof propertyValue === "number") {
			return this.numberMatches(filterValue, propertyValue);
		} else if (typeof propertyValue === "boolean") {
            return this.boolMatches(filterValue, propertyValue);
        }

        return false;
    }

    public static stringMatches(filterValue?: string | null, propertyValue?: string | null) {
        // avoid null and undefined
		filterValue = (filterValue || "").toLowerCase();
		propertyValue = (propertyValue || "").toLowerCase();
		return propertyValue.indexOf(filterValue) >= 0;
	}

    public static numberMatches(filterValue?: string | null, propertyValue?: number | null) {
        // avoid null and undefined
        filterValue = filterValue || "";
        if (filterValue === "") {
			return true;
		}

        const value = propertyValue || "";
        const parts = filterValue.toLowerCase().split(',');
        return parts.findIndex(v => v === value.toString()) >= 0;
    }

    public static boolMatches(filterValue?: string | null, propertyValue?: boolean | null) {
        // avoid null and undefined
        filterValue = filterValue || "";
        if (filterValue === "") {
			return true;
		}

        const value = (propertyValue || false).toString().toLowerCase().trim();
        return filterValue.toLowerCase().trim() === value;
    }
}