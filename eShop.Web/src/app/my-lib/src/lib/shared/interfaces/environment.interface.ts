export interface IEnvironment {
    production: boolean;
    api: string;
    baseUrlCore: string;
    clientId: string;
    clientSecret: string;
    scopes: string;
    firebase: IFirebaseConfig;
}
export interface IFirebaseConfig {
    apiKey: string;
    authDomain: string;
    projectId: string;
    storageBucket: string;
    messagingSenderId: string;
    appId: string;
    measurementId?: string;
}