namespace Cinema.Core;

public class ReservationService
{
    private readonly IReservationRepository _repo;
    private readonly ICinemaViewingRepository _cvRepo;
    public ReservationService(IReservationRepository reservationRepository, ICinemaViewingRepository cvRepo)
    {
        _repo = reservationRepository;

        _cvRepo = cvRepo;
    }

    public async Task<Reservation> AddReservationAsync(Reservation r)
    {
        if (r != null)
        {
            List<Reservation> listOfReservationsForSpecifiedCinemaV = await _repo.GetReservationsForCinemaViewingAsync(r.CinemaViewingId);
            var totalReservedPlaces = listOfReservationsForSpecifiedCinemaV.Sum(res => res.Quantity);

            List<CinemaViewing> listOfCinemaViewings = await _cvRepo.GetAllCinemaViewingsAsync();
            CinemaViewing? cinemaViewing = listOfCinemaViewings.Where(cv => cv.Id == r.CinemaViewingId).FirstOrDefault();

            if (cinemaViewing != null)
            {
                if (r.Quantity <= (cinemaViewing.PlaceQuantity - totalReservedPlaces))
                {
                    return await _repo.AddReservationAsync(r);
                }
                else
                {
                    throw new Exception("There are not enough seats available for this reservation");
                }
            }
            throw new KeyNotFoundException("CinemaViewing with the specified ID was not found");
        }
        throw new ArgumentNullException(nameof(r), "Reservation object cannot be null.");
    }

    public async Task<List<Reservation>> GetAllReservationsAsync()
    {
        List<Reservation> reservations = await _repo.GetAllReservationsAsync();
        if (reservations != null)
        {
            return reservations;
        }
        return null;
    }

    public async Task<List<Reservation>> GetReservationsForCinemaViewingAsync(int cinemaViewingId)
    {
        if (await _repo.GetReservationsForCinemaViewingAsync(cinemaViewingId) != null)
        {
            return await _repo.GetReservationsForCinemaViewingAsync(cinemaViewingId);
        }
        return null;
    }

    public async Task<Reservation> DeleteReservationAsync(int id)
    {
        if (id != null)
        {
            Reservation deletedR = await _repo.DeleteReservationAsync(id);
            if (deletedR != null)
            {
                return deletedR;
            }
            else
            {
                throw new KeyNotFoundException("Reservation with the specified ID was not found");
            }
        }
        throw new ArgumentNullException("Reservation Id cannot be null.");
    }

    public void GenerateReservationCode(Reservation r)
    {
        r.ReservationCode = Guid.NewGuid().ToString();
    }
}