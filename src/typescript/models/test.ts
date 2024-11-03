import { Range } from "vscode";

export interface Test {
    fullName: string | null;
    name: string;
    filePath: string;
    range: Range | null;
    children: Test[] | null;
}

export interface TestResult {
    fullName: string;
    duration: string;
    state: string;
    errorMessage: string | null;
}