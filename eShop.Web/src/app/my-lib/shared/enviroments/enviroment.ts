import { IEnvironment } from "../../src/lib/shared/interfaces/environment.interface";

export const environment: IEnvironment = {
    production: false,
    api: 'https://localhost:5187',
    baseUrlCore: 'https://localhost:5187',
    clientId: 'client-web',
    // Public browser clients must not ship a client secret; use PKCE instead.
    clientSecret: '',
    scopes: 'offline_access'
}