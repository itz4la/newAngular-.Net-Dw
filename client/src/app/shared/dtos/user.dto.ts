export interface UserDto {
  id: string;
  userName: string;
  email: string;
  role: string;
}

export interface CreateAdminUserDto {
  userName: string;
  email: string;
  password: string;
}

export interface UpdateUserDto {
  userName: string;
  email: string;
  role: string;
}

export interface UserFilterDto {
  userName?: string;
  role?: string;
  pageNumber: number;
  pageSize: number;
}

