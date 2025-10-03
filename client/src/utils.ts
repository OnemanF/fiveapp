export default function UpdateList<T>( list: T[], updatedItem: T, keyFn: (item: T) => string): T[] {
    const index = list.findIndex(i => keyFn(i) === keyFn(updatedItem));
    if (index === -1) {
        return [...list, updatedItem];
    } else {
        const newList = [...list];
        newList[index] = updatedItem;
        return newList;
    }

}