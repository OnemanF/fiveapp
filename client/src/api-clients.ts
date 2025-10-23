import {LibraryClient} from "./generated-client.ts";

const isProduction = import.meta.env.PROD;

const prod = "https://server-frosty-darkness-1271.fly.dev";
const dev = "http://localhost:5114";

export const finalUrl = isProduction ? prod : dev;

export const libraryApi = new LibraryClient(finalUrl);
