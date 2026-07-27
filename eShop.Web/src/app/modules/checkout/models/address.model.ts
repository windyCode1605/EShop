export interface Address {
    id: number;
    receiverName: string;
    receiverPhone: string;
    street: string;
    city: string;
    province: string;
    isDefault: boolean;
}

export interface AddressCreatDto {
    receiverName: string;
    receiverPhone: string;
    street: string;
    city: string;
    province: string;
}