export abstract class Utils {         

    public static matchesFilter(searchText: string | null, ...values: (string | null | undefined)[]): boolean {

        if (!searchText || searchText.trim() === "")
            return true;
        var items = values.filter(item => item != null && item > '');
        if (items.length < 1)
            return false;
        return items.find(item => item!.toLowerCase().indexOf(searchText.trim().toLowerCase()) >= 0) != null;
    }

    public static validateIPaddress(ipAddress: string) {  

        return (/^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/.test(ipAddress));
    }  
}