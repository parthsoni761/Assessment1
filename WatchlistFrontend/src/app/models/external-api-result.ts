// This interface defines the shape of the data coming from your backend's OMDb endpoints.
export interface ExternalApiResultDto {
  title: string;
  itemType: string; // "Movie" or "TV Show"
  releaseYear?: number;
  genre: string;
}
