import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { WeeklySchedule } from './components/WeeklySchedule';
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/groups-schedule',
    element: <WeeklySchedule key='groups' isGroup={true} />
  },
  {
    path: '/teachers-schedule',
    element: <WeeklySchedule key='teachers' isGroup={false} />
  },
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
