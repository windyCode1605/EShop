import { IEnvironment } from "../../src/lib/shared/interfaces/environment.interface";

export const environment: IEnvironment = {
    production: false,
    api: 'http://localhost:5178',
    baseUrlCore: 'http://localhost:5178',
    clientId: 'client-web',
    clientSecret: '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',
    scopes: 'offline_access',
    firebase: {
        apiKey: "AIzaSyAKoAu5qPLcI7ir8Jk0-wKxamHERQGNYrA",
        authDomain: "eshop-c06ad.firebaseapp.com",
        projectId: "eshop-c06ad",
        storageBucket: "eshop-c06ad.firebasestorage.app",
        messagingSenderId: "602858569079",
        appId: "1:602858569079:web:44eca50b8874b3c89cf572",
        measurementId: "G-3V5M2VFY1W"
    }
}