import {useAtom} from "jotai";
import {AllAuthorsAtom, AllBooksAtom} from "../atoms/atoms.ts";
import {ApiException, type AuthorDto, type BookDto} from "../generated-client.ts";
import {libraryApi} from "../api-clients.ts";
import type {BookProps} from "./Books.tsx";
//import UpdateList from "../utils.ts";
import {useState} from "react";
import type {ProblemDetails} from "../problemdetails.ts";
import toast from "react-hot-toast";

export function Book(props: BookProps) {

    const [books, setBooks] = useAtom(AllBooksAtom);
    const [authors] = useAtom(AllAuthorsAtom);
    const [newAuthorsIdsForBook, setNewAuthorsIdsForBook] = useState<string[]>(props.book.authorsIds || []);

    function getAuthorNamesFromIds(ids: string[]): string[] {
        const filtered = authors.filter(a => ids.includes(a.id!));
        const names = filtered.map(f => f.name!);
        return names;
    }

    function updateBook(author: AuthorDto, book: BookDto) {
        if(!newAuthorsIdsForBook.includes(author.id)) {
            const duplicate = [...newAuthorsIdsForBook];
            duplicate.push(author.id!);
            setNewAuthorsIdsForBook(duplicate);
        } else {
            const index = newAuthorsIdsForBook.indexOf(author.id!);
            if (index > -1) {
                const duplicate = [...newAuthorsIdsForBook].splice(index, 1);
                setNewAuthorsIdsForBook(duplicate);
            }
        }

        libraryApi.updateBook({
            authorsIds: newAuthorsIdsForBook,
            bookIdForLookupReference: book.id!,
            genreId: book.genre!.id,
            newTitle: book.title!,
            newPageCount: book.pages!
        }).then(r => {
            const duplicate = [...books]
            const index = books.findIndex(b => b.id === r.id);
            if(index > -1) {
                duplicate[index] = r;
                setBooks(duplicate);
                toast("Book updated succesfully")
            }
        }).catch(e => {
            if(e instanceof ApiException) {
                console.log(JSON.stringify(e))
                const problemDetails = JSON.parse(e.response) as ProblemDetails;
                toast(problemDetails.title)
            }
        })
    }

    return <li className="p-5 list-row w-full flex justify-between">


        <div>
            <div>{props.book.title}</div>
        <div className="text-xs uppercase font-semibold opacity-60">Pages: {props.book.pages}</div>
    <div
    className="text-xs uppercase font-semibold opacity-60">By {getAuthorNamesFromIds(props.book.authorsIds!)}</div>

    </div>
    <details className="dropdown dropdown-left">
    <summary className="btn m-1">⚙️</summary>
    <ul className="menu dropdown-content bg-base-100 rounded-box z-1 w-52 p-2 shadow-sm">
        <li>Assign author to book</li>
    {
        authors.map(a => <div className="join">
        <p className="join-item btn btn-disabled">{a.name}</p><input className="checkbox join-item checkbox-lg" type="checkbox" checked={props.book.authorsIds.includes(a.id)} onClick={() => updateBook(a, props.book)} />
            </div>)
    }
    </ul>
    </details>
    </li>;

}