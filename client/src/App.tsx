import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Home from "./Components/Home";
import { DevTools } from "jotai-devtools";
import { useEffect } from "react";
import { libraryApi } from "./api-clients";
import { useAtom } from "jotai";
import "jotai-devtools/styles.css";
import { AllAuthorsAtom, AllBooksAtom, AllGenresAtom } from "./atoms/atoms";

// ✅ import your components as *default* exports
import Books from "./Components/Books";
import Authors from "./Components/Authors";
import Genres from "./Components/Genres";

import toast, { Toaster } from "react-hot-toast";

// ✅ remove SwaggerException if your generated-client doesn’t export it
// (NSwag only adds it if "wrapDtoExceptions" or "generateResponseClasses" is true)
function App() {
    const [, setAuthors] = useAtom(AllAuthorsAtom);
    const [, setBooks] = useAtom(AllBooksAtom);
    const [, setGenres] = useAtom(AllGenresAtom);

    useEffect(() => {
        initializeData();
    }, []);

    async function initializeData() {
        try {
            setAuthors(await libraryApi.getAuthors());
            setBooks(await libraryApi.getBooks());
            setGenres(await libraryApi.getGenres());
        } catch (e) {
            // ✅ explicitly type e as unknown, then narrow to Error
            if (e instanceof Error) {
                toast.error(e.message);
            } else {
                toast.error("Error fetching data");
            }
        }
    }

    return (
        <>
            <RouterProvider
                router={createBrowserRouter([
                    {
                        path: "",
                        element: <Home />,
                        children: [
                            { path: "books", element: <Books /> },
                            { path: "authors", element: <Authors /> },
                            { path: "genres", element: <Genres /> },
                        ],
                    },
                ])}
            />
            <DevTools />
            <Toaster position="top-center" />
        </>
    );
}

export default App;
