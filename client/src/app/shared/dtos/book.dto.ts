export interface BookDto {
  id: number;
  title: string;
  author: string;
  description: string;
  genreId: number;
  genreName: string;
  coverImageUrl: string;
  publishedDate: string;
  isAvailable: boolean;
}

export interface CreateBookDto {
  title: string;
  author: string;
  description?: string;
  genreId: number;
  coverImageUrl?: string;
  publishedDate: string;
}

export interface UpdateBookDto {
  title: string;
  author: string;
  description?: string;
  genreId: number;
  coverImageUrl?: string;
  publishedDate: string;
}

export interface BookFilterDto {
  title?: string;
  author?: string;
  genreId?: number;
  pageNumber: number;
  pageSize: number;
}
