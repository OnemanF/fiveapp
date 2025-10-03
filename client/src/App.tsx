import {createBrowserRouter, RouterProvider} from "react-router";
import Home from "./Components/Home";
import {DevTools} from "jotai-devtools";
import {useEffect} from "react";
import {libraryApi} from "./api-clients.ts";
import {useAtom} from "jotai";
import 'jotai-devtools/styles.css'
import {AllAuthorsAtom, AllBooksAtom, AllGenresAtom} from "./atoms/atoms.ts";
import Books from "./Components/Books";
import Authors from "./Components/Authors";
import Genres from "./Components/Genres";
import toast, {Toaster} from "react-hot-toast";
import {ApiException} from "./generated-client.ts";
import type {ProblemDetails} from "./problemdetails.ts";


function App() {

    const [, setAuthors] = useAtom(AllAuthorsAtom)
    const [, setBooks] = useAtom(AllBooksAtom)
    const [, setGenres] = useAtom(AllGenresAtom)

    useEffect(() => {
        initializeData();
    }, [])

    async function initializeData() {
        try {
            setAuthors(await libraryApi.getAuthors());
            setBooks(await libraryApi.getBooks())
            setGenres(await libraryApi.getGenres())
        } catch (e) {
            if (e instanceof ApiException) {
                console.log(JSON.stringify(e))
                const problemDetails = JSON.parse(e.response) as ProblemDetails;
                toast(problemDetails.title)
            } else {
                toast.error("Error getting data from server")
            }

        }
    }



    return (
        <>
            <RouterProvider router={createBrowserRouter([
                {
                    path: '',
                    element: <Home/>,
                    children: [
                        {
                            path: 'books',
                            element: <Books/>
                        },
                        {
                            path: 'authors',
                            element: <Authors/>
                        },
                        {
                            path: 'genres',
                            element: <Genres/>
                        }
                    ]
                }
            ])}/>
            <DevTools/>
            <Toaster
                position="top-center"
                reverseOrder={false}
            />
        </>
    )
}

export default App
