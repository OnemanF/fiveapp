import { useEffect, useState } from "react";
import { libraryApi } from "../api-clients";
import {type AuthorDto, CreateAuthorRequestDto} from "../generated-client";
import toast from "react-hot-toast";

export default function Authors() {
    const [authors, setAuthors] = useState<AuthorDto[]>([]);
    const [newAuthor, setNewAuthor] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchAuthors();
    }, []);

    async function fetchAuthors() {
        try {
            setLoading(true);
            const data = await libraryApi.getAuthors();
            setAuthors(data);
        } catch {
            toast.error("Failed to load authors");
        } finally {
            setLoading(false);
        }
    }

    async function handleCreate() {
        if (!newAuthor.trim()) return toast.error("Author name required");
        
        try {
            const dto = new CreateAuthorRequestDto();
            dto.name = newAuthor;
            await libraryApi.createAuthor(dto);
            toast.success("✅ Author added");
            setNewAuthor("");
            await fetchAuthors();
        } catch {
            toast.error("Failed to create author");
        }
    }

    async function handleDelete(id: string) {
        if (!confirm("Are you sure you want to delete this author?")) return;
        try {
            await libraryApi.deleteAuthor(id);
            toast.success("🗑️ Author deleted");
            await fetchAuthors();
        } catch {
            toast.error("Failed to delete author");
        }
    }

    return (
        <div className="p-6 space-y-8">
            <h1 className="text-2xl font-semibold text-indigo-700">✍️ Author Management</h1>

            {/* CREATE AUTHOR */}
            <div className="bg-white p-4 rounded-lg shadow border border-gray-200 flex flex-wrap items-center gap-3">
                <input
                    value={newAuthor}
                    onChange={(e) => setNewAuthor(e.target.value)}
                    placeholder="Enter author name"
                    className="border border-gray-300 rounded-md px-2 py-1 text-sm w-56 focus:ring-1 focus:ring-indigo-400"
                />
                <button
                    onClick={handleCreate}
                    className="px-4 py-1.5 bg-indigo-600 text-white text-sm rounded-md hover:bg-indigo-700 transition"
                >
                    Create
                </button>
            </div>

            {/* AUTHORS LIST */}
            <div>
                <h2 className="text-xl font-medium text-gray-800 mb-2">Existing Authors</h2>
                {loading ? (
                    <p>Loading...</p>
                ) : authors.length === 0 ? (
                    <p className="text-gray-500 text-sm">No authors found.</p>
                ) : (
                    <ul className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
                        {authors.map((a) => (
                            <li
                                key={a.id}
                                className="bg-white p-3 rounded-lg border border-gray-200 shadow-sm hover:shadow transition flex justify-between items-center"
                            >
                                <div>
                                    <h3 className="font-semibold text-indigo-700 text-sm">{a.name}</h3>
                                    <p className="text-xs text-gray-500">
                                        ID: <span className="text-gray-400">{a.id}</span>
                                    </p>
                                </div>
                                <button
                                    onClick={() => handleDelete(a.id!)}
                                    className="px-3 py-1 bg-red-500 hover:bg-red-600 text-white text-xs rounded-md transition"
                                >
                                    Delete
                                </button>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}
