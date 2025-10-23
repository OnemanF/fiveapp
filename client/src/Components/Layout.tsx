import { NavLink, Outlet } from "react-router-dom";
import type { ReactNode } from "react";

interface LayoutProps {
    children?: ReactNode;
}

export default function Layout({ children }: LayoutProps) {
    return (
        <div className="flex min-h-screen bg-gray-100 text-indigo-700">
            <aside className="w-64 bg-indigo-700 shadow-xl flex flex-col p-6">
                <h1 className="text-2xl font-bold text-white mb-8 tracking-tight">
                    📚 Library Admin
                </h1>
                <nav className="flex flex-col space-y-2">
                    {[
                        { to: "/books", label: "Books" },
                        { to: "/authors", label: "Authors" },
                        { to: "/genres", label: "Genres" },
                    ].map(({ to, label }) => (
                        <NavLink
                            key={to}
                            to={to}
                            className={({ isActive }) =>
                                `px-3 py-2 rounded-lg transition font-medium ${
                                    isActive
                                        ? "bg-indigo-500 text-white"
                                        : "text-indigo-100 hover:bg-indigo-600 hover:text-white"
                                }`
                            }
                        >
                            {label}
                        </NavLink>
                    ))}
                </nav>
            </aside>

            <main className="flex-1 p-10 bg-gray-50 overflow-y-auto">
                {children ?? <Outlet />}
            </main>
        </div>
    );
}
