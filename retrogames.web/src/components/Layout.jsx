
import { Link, Outlet } from 'react-router-dom'

function Layout() {
  return (
    <div className="d-flex flex-column min-vh-100">
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand" to="/">
            RetroGames
          </Link>

          <div className="navbar-nav">
            <Link className="nav-link" to="/">
              Catálogo
            </Link>
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        <Outlet />
      </main>

      <footer className="bg-dark text-white text-center py-3">
        <div className="container">
          RetroGames
        </div>
      </footer>
    </div>
  )
}

export default Layout
