import { useEffect, useState } from "react";
import {
    CreateBookRequestDto,
    UpdateBookRequestDto,
    type BookDto,
    type GenreDto,
    type AuthorDto,
} from "../generated-client";
import { libraryApi } from "../api-clients";
import toast from "react-hot-toast";

export default function Books() {
    const [books, setBooks] = useState<BookDto[]>([]);
    const [genres, setGenres] = useState<GenreDto[]>([]);
    const [authors, setAuthors] = useState<AuthorDto[]>([]);
    const [loading, setLoading] = useState(true);

    const [title, setTitle] = useState("");
    const [pages, setPages] = useState<number>(0);
    const [genreId, setGenreId] = useState<string>("");
    const [authorsIds, setAuthorsIds] = useState<string[]>([]);
    const [editingBook, setEditingBook] = useState<BookDto | null>(null);

    useEffect(() => {
        loadAll();
    }, []);

    async function loadAll() {
        try {
            setLoading(true);
            const [booksData, authorsData, genresData] = await Promise.all([
                libraryApi.getBooks(),
                libraryApi.getAuthors(),
                libraryApi.getGenres(),
            ]);
            setBooks(booksData);
            setAuthors(authorsData);
            setGenres(genresData);
        } catch {
            toast.error("Failed to load data");
        } finally {
            setLoading(false);
        }
    }

    function resetForm() {
        setTitle("");
        setPages(0);
        setGenreId("");
        setAuthorsIds([]);
        setEditingBook(null);
    }

    async function createBook() {
        if (!title.trim() || pages <= 0) {
            toast.error("Enter valid title and page count");
            return;
        }

        const dto = new CreateBookRequestDto();
        dto.title = title;
        dto.pages = pages;
        dto.genreId = genreId || undefined;
        dto.authorsIds = authorsIds;

        try {
            const created = await libraryApi.createBook(dto);
            setBooks([...books, created]);
            resetForm();
            toast.success("✅ Book created!");
        } catch {
            toast.error("Failed to create book");
        }
    }

    function startEditing(book: BookDto) {
        setEditingBook(book);
        setTitle(book.title ?? "");
        setPages(book.pages ?? 0);
        setGenreId(book.genre?.id ?? "");
        setAuthorsIds(book.authorsIds ?? []);
    }

    async function updateBook() {
        if (!editingBook) return;

        const dto = new UpdateBookRequestDto();
        dto.bookIdForLookupReference = editingBook.id!;
        dto.newTitle = title;
        dto.newPageCount = pages;
        dto.genreId = genreId || undefined;
        dto.authorsIds = authorsIds;

        try {
            const updated = await libraryApi.updateBook(dto);
            setBooks(books.map((b) => (b.id === updated.id ? updated : b)));
            toast.success("✏️ Book updated!");
            resetForm();
        } catch {
            toast.error("Failed to update book");
        }
    }

    async function deleteBook(id: string) {
        if (!confirm("Are you sure you want to delete this book?")) return;
        try {
            await libraryApi.deleteBook(id);
            setBooks(books.filter((b) => b.id !== id));
            toast.success("🗑️ Book deleted");
        } catch {
            toast.error("Failed to delete book");
        }
    }

    return (
        <div className="p-6 space-y-8">
            <h1 className="text-2xl font-semibold text-indigo-700">📚 Book Management</h1>

            {/* CREATE / EDIT FORM */}
            <div className="bg-white p-4 rounded-lg shadow border border-gray-200">
                <h2 className="text-lg font-medium mb-4">
                    {editingBook ? "✏️ Edit Book" : "➕ Create New Book"}
                </h2>

                <div className="flex flex-wrap gap-3 items-center">
                    <input
                        className="border border-gray-300 rounded-md px-2 py-1 text-sm w-40 focus:ring-1 focus:ring-indigo-400"
                        placeholder="Title"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />
                    <input
                        className="border border-gray-300 rounded-md px-2 py-1 text-sm w-24 focus:ring-1 focus:ring-indigo-400"
                        placeholder="Pages"
                        type="number"
                        value={pages}
                        onChange={(e) => setPages(Number(e.target.value))}
                    />
                    <select
                        className="border border-gray-300 rounded-md px-2 py-1 text-sm w-36 focus:ring-1 focus:ring-indigo-400"
                        value={genreId}
                        onChange={(e) => setGenreId(e.target.value)}
                    >
                        <option value="">Genre</option>
                        {genres.map((g) => (
                            <option key={g.id} value={g.id}>
                                {g.name}
                            </option>
                        ))}
                    </select>

                    <div className="flex flex-col w-56">
                        <label className="text-xs text-gray-500 mb-1">Authors</label>
                        <select
                            multiple
                            className="border border-gray-300 rounded-md px-2 py-1 text-sm h-20 focus:ring-1 focus:ring-indigo-400"
                            value={authorsIds}
                            onChange={(e) =>
                                setAuthorsIds(
                                    Array.from(e.target.selectedOptions, (o) => o.value)
                                )
                            }
                        >
                            {authors.map((a) => (
                                <option key={a.id} value={a.id}>
                                    {a.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    {editingBook ? (
                        <>
                            <button
                                onClick={updateBook}
                                className="px-4 py-1.5 bg-yellow-500 text-white rounded-md hover:bg-yellow-600 text-sm transition"
                            >
                                Update
                            </button>
                            <button
                                onClick={resetForm}
                                className="px-4 py-1.5 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400 text-sm transition"
                            >
                                Cancel
                            </button>
                        </>
                    ) : (
                        <button
                            onClick={createBook}
                            className="px-4 py-1.5 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 text-sm transition"
                        >
                            Create
                        </button>
                    )}
                </div>
            </div>

            {/* BOOK LIST */}
            <div>
                <h2 className="text-xl font-medium text-gray-800 mb-2">Existing Books</h2>
                {loading ? (
                    <p>Loading...</p>
                ) : books.length === 0 ? (
                    <p>No books found.</p>
                ) : (
                    <ul className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {books.map((b) => (
                            <li
                                key={b.id}
                                className="bg-white p-3 rounded-lg border border-gray-200 shadow-sm hover:shadow transition"
                            >
                                <div className="flex justify-between items-start">
                                    <div>
                                        <h3 className="font-semibold text-indigo-700 text-sm">
                                            {b.title}
                                        </h3>
                                        <p className="text-xs text-gray-600">{b.pages} pages</p>
                                        {b.genre && (
                                            <p className="text-xs text-gray-500">
                                                Genre: {b.genre.name}
                                            </p>
                                        )}
                                        <p className="text-xs text-gray-500 mt-1">
                                            Authors:{" "}
                                            {b.authors?.map((a) => a.name).join(", ") || "None"}
                                        </p>
                                    </div>
                                    <div className="flex gap-1.5">
                                        <button
                                            onClick={() => startEditing(b)}
                                            className="px-2 py-0.5 bg-yellow-400 hover:bg-yellow-500 text-xs rounded text-white transition"
                                        >
                                            Edit
                                        </button>
                                        <button
                                            onClick={() => deleteBook(b.id!)}
                                            className="px-2 py-0.5 bg-red-500 hover:bg-red-600 text-xs rounded text-white transition"
                                        >
                                            Delete
                                        </button>
                                    </div>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}
