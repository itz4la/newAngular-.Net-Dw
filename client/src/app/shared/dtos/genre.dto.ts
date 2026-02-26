export interface GenreDto {
  id: number;
  name: string;
  booksCount: number;
}

export interface CreateGenreDto {
  name: string;
}

export interface UpdateGenreDto {
  name: string;
}

export interface GenreFilterDto {
  name?: string;
  pageNumber: number;
  pageSize: number;
}
