import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from "../../my-lib/shared/enviroments/enviroment";

export const apiUrlInterceptor: HttpInterceptorFn = (req, next) => {
    const isAbsoluteUrl = req.url.startsWith('http')
    if (!isAbsoluteUrl) {
        const apiReq = req.clone({ url: `${environment.api}${req.url}` });
        return next(apiReq);
    }
    return next(req);
}