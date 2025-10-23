import { useEffect, useState } from "react";
import { libraryApi } from "../api-clients";
import type { GenreDto } from "../generated-client";
import { CreateGenreDto } from "../generated-client";
import toast from "react-hot-toast";

export default function Genres() {
    const [genres, setGenres] = useState<GenreDto[]>([]);
    const [newGenre, setNewGenre] = useState("");
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchGenres();
    }, []);

    async function fetchGenres() {
        try {
            setLoading(true);
            const data = await libraryApi.getGenres();
            setGenres(data);
        } catch {
            toast.error("Failed to load genres");
        } finally {
            setLoading(false);
        }
    }

    async function handleCreate() {
        if (!newGenre.trim()) return toast.error("Genre name required");

        try {
            const dto = new CreateGenreDto();
            dto.name = newGenre;
            await libraryApi.createGenre(dto);
            toast.success("✅ Genre created!");
            setNewGenre("");
            await fetchGenres();
        } catch {
            toast.error("Failed to create genre");
        }
    }

    async function handleDelete(id: string) {
        if (!confirm("Delete this genre?")) return;
        try {
            await libraryApi.deleteGenre(id);
            toast.success("🗑️ Genre deleted");
            await fetchGenres();
        } catch {
            toast.error("Failed to delete genre");
        }
    }

    return (
        <div className="p-6 space-y-8">
            <h1 className="text-2xl font-semibold text-indigo-700">🎨 Genre Management</h1>

            {/* CREATE GENRE */}
            <div className="bg-white p-4 rounded-lg shadow border border-gray-200 flex flex-wrap items-center gap-3">
                <input
                    value={newGenre}
                    onChange={(e) => setNewGenre(e.target.value)}
                    placeholder="Enter genre name"
                    className="border border-gray-300 rounded-md px-2 py-1 text-sm w-56 focus:ring-1 focus:ring-indigo-400"
                />
                <button
                    onClick={handleCreate}
                    className="px-4 py-1.5 bg-indigo-600 text-white text-sm rounded-md hover:bg-indigo-700 transition"
                >
                    Create
                </button>
            </div>

            {/* GENRES LIST */}
            <div>
                <h2 className="text-xl font-medium text-gray-800 mb-2">Existing Genres</h2>
                {loading ? (
                    <p>Loading...</p>
                ) : genres.length === 0 ? (
                    <p className="text-gray-500 text-sm">No genres found.</p>
                ) : (
                    <ul className="grid sm:grid-cols-2 lg:grid-cols-3 gap-4">
                        {genres.map((g) => (
                            <li
                                key={g.id}
                                className="bg-white p-3 rounded-lg border border-gray-200 shadow-sm hover:shadow transition flex justify-between items-center"
                            >
                                <div>
                                    <h3 className="font-semibold text-indigo-700 text-sm">
                                        {g.name}
                                    </h3>
                                    <p className="text-xs text-gray-500">
                                        ID: <span className="text-gray-400">{g.id}</span>
                                    </p>
                                </div>
                                <button
                                    onClick={() => handleDelete(g.id!)}
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
