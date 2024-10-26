export abstract class Utils {         

    public static matchesFilter(searchText: string | null, ...values: (string | null | undefined)[]): boolean {

        if (!searchText || searchText.trim() === "")
            return true;
        var items = values.filter(item => item != null && item > '');
        if (items.length < 1)
            return false;
        return items.find(item => item!.toLowerCase().indexOf(searchText.trim().toLowerCase()) >= 0) != null;
    }
}