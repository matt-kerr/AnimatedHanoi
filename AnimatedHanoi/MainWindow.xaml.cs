// Matthew Kerr
// March 11, 2015

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;

namespace AnimatedHanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int x_offset;
        public static int y_offset_up;
        public static int y_offset_down;
        public static int num_pegs;
        public static int[,] moves;
        public static int total_moves;
        public static int moves_taken;
        public static int run_number;
        public static Rectangle curr;
        public static Rectangle[] all_rects;
        public static Stack<Rectangle> stack0;
        public static Stack<Rectangle> stack1;
        public static Stack<Rectangle> stack2;
        public static Stack<Rectangle>[] all_stacks;
        public static double animation_speed;
        public static Storyboard pathAnimationStoryboard;
        public static bool paused;
        
        public MainWindow()
        {
            InitializeComponent();
            NameScope.SetNameScope(this, new NameScope());
            this.Content = canvas_main;

            run_number = 0;
            rect0.Visibility = Visibility.Hidden;
            rect1.Visibility = Visibility.Hidden;
            rect2.Visibility = Visibility.Hidden;
            rect3.Visibility = Visibility.Hidden;
            rect4.Visibility = Visibility.Hidden;
            rect5.Visibility = Visibility.Hidden;
            rect6.Visibility = Visibility.Hidden;
            rect7.Visibility = Visibility.Hidden;
            rect_peg0_base.Visibility = Visibility.Hidden;
            rect_peg1_base.Visibility = Visibility.Hidden;
            rect_peg2_base.Visibility = Visibility.Hidden;
            rect_peg0_spool.Visibility = Visibility.Hidden;
            rect_peg1_spool.Visibility = Visibility.Hidden;
            rect_peg2_spool.Visibility = Visibility.Hidden;
            lbl_0.Visibility = Visibility.Hidden;
            lbl_1.Visibility = Visibility.Hidden;
            lbl_2.Visibility = Visibility.Hidden;
            btn_start.Visibility = Visibility.Visible;
            mnu_start.IsEnabled = true;
            btn_pause.Visibility = Visibility.Hidden;
            mnu_pause.IsEnabled = false;
            btn_abort.Visibility = Visibility.Hidden;
            mnu_abort.IsEnabled = false;
            lbl_moves.Visibility = Visibility.Hidden;
            textBx_moves.Visibility = Visibility.Hidden;
            paused = false;
            
            
            x_offset = 0;
            y_offset_up = 0;
            y_offset_down = 0;
            num_pegs = 5;
            animation_speed = 1.0;

        }

        public void MovePeg(int source, int destination)
        {
            y_offset_up = -1 * (150 + (120 - (all_stacks[source].Count * 15)));
            x_offset = 150 * (destination - source);
            y_offset_down = 15 * (all_stacks[source].Count - 1 - all_stacks[destination].Count);

            curr = all_stacks[source].Pop();
            all_stacks[destination].Push(curr);
            AnimateRectangle();
        }

        public void Hanoi(int numDisks, int source, int destination, int intermediate)
        {
            if (numDisks == 1)
            {
                AddMove(source, destination);
            }
            else
            {
                Hanoi(numDisks - 1, source, intermediate, destination);
                AddMove(source, destination);
                Hanoi(numDisks - 1, intermediate, destination, source);
            }
        }

        public void PerformMove()
        {
            if (moves_taken < total_moves)
            {
                textBx_moves.Text += "Move a disk from " + moves[moves_taken, 0] + " to " + moves[moves_taken, 1] + "\n";
                textBx_moves.ScrollToEnd();
                MovePeg(moves[moves_taken, 0], moves[moves_taken, 1]);
            }
            else
            {
                btn_abort.Content = "Reset";
                mnu_abort.Header = "Reset";
                btn_pause.Visibility = Visibility.Hidden;
                mnu_pause.IsEnabled = false;
            }

        }

        public void InitializeMoves()
        {
            moves = new int[255, 2];
            for (int i = 0; i < 255; i++)
            {
                moves[i, 0] = -1;
                moves[i, 1] = -1;
            }
            total_moves = 0;
            moves_taken = 0;
        }

        public void AddMove(int source, int destination)
        {
            moves[total_moves, 0] = source;
            moves[total_moves, 1] = destination;
            total_moves++;
        }

        public void InitializeRectangles()
        {
            Random rnd = new Random();
            SolidColorBrush brush;
            all_rects = new Rectangle[8];
            all_rects[0] = rect0;
            all_rects[1] = rect1;
            all_rects[2] = rect2;
            all_rects[3] = rect3;
            all_rects[4] = rect4;
            all_rects[5] = rect5;
            all_rects[6] = rect6;
            all_rects[7] = rect7;
            for (int i = 0; i < all_rects.Length; i++)
            {

                all_rects[i].Visibility = Visibility.Hidden;
                brush = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255)));
                all_rects[i].Fill = brush;
            }
            
        }

        public void InitializeSpools()
        {
            rect_peg0_spool.Height = 15 + (num_pegs * 15);
            rect_peg1_spool.Height = 15 + (num_pegs * 15);
            rect_peg2_spool.Height = 15 + (num_pegs * 15);
            double offset = ((8 - num_pegs) * 15);
            Canvas.SetTop(rect_peg0_spool, 120 + offset);
            Canvas.SetTop(rect_peg1_spool, 120 + offset);
            Canvas.SetTop(rect_peg2_spool, 120 + offset);

            rect_peg0_base.Width = 20 + (num_pegs * 10);
            rect_peg1_base.Width = 20 + (num_pegs * 10);
            rect_peg2_base.Width = 20 + (num_pegs * 10);
            offset = (100 - rect_peg0_base.Width) / 2;
            Canvas.SetLeft(rect_peg0_base, 50 + offset);
            Canvas.SetLeft(rect_peg1_base, 200 + offset);
            Canvas.SetLeft(rect_peg2_base, 350 + offset);

            rect_peg0_base.Visibility = Visibility.Visible;
            rect_peg1_base.Visibility = Visibility.Visible;
            rect_peg2_base.Visibility = Visibility.Visible;
            rect_peg0_spool.Visibility = Visibility.Visible;
            rect_peg1_spool.Visibility = Visibility.Visible;
            rect_peg2_spool.Visibility = Visibility.Visible;
        }

        public void InitializeStacks()
        {
            

            stack0 = new Stack<Rectangle>();
            stack1 = new Stack<Rectangle>();
            stack2 = new Stack<Rectangle>();

            all_stacks = new Stack<Rectangle>[3];
            all_stacks[0] = stack0;
            all_stacks[1] = stack1;
            all_stacks[2] = stack2;
            int temp = 0;
            for (int i = num_pegs - 1; i >= 0; i--)
            {
                curr = all_rects[i];
                stack0.Push(curr);
                curr.Visibility = Visibility.Visible;
                Vector offset = VisualTreeHelper.GetOffset(curr);
                var top = Canvas.GetTop(curr);
                var left = Canvas.GetLeft(curr);
                Canvas.SetLeft(curr, 45 + ((8 - i) * 5));
                Canvas.SetTop(curr, 235 - (temp * 15));
                temp++;
            }
            
        }

        private void StartPuzzle()
        {
            run_number++;
            InitializeRectangles();
            InitializeMoves();
            InitializeStacks();
            InitializeSpools();
            btn_abort.Content = "Abort";
            mnu_abort.Header = "Abort";
            lbl_numDisks.Visibility = Visibility.Hidden;
            slider_numDisks.Visibility = Visibility.Hidden;
            lbl_animationSpeed.Visibility = Visibility.Hidden;
            radioBtn_slow.Visibility = Visibility.Hidden;
            radioBtn_normal.Visibility = Visibility.Hidden;
            radioBtn_fast.Visibility = Visibility.Hidden;
            mnu_start.IsEnabled = false;
            mnu_abort.IsEnabled = true;
            btn_start.Visibility = Visibility.Hidden;
            mnu_start.IsEnabled = false;
            btn_pause.Visibility = Visibility.Visible;
            mnu_pause.IsEnabled = true;
            btn_abort.Visibility = Visibility.Visible;
            mnu_abort.IsEnabled = true;
            lbl_moves.Visibility = Visibility.Visible;
            textBx_moves.Visibility = Visibility.Visible;
            lbl_0.Visibility = Visibility.Visible;
            lbl_1.Visibility = Visibility.Visible;
            lbl_2.Visibility = Visibility.Visible;
            
            Hanoi(num_pegs, 0, 2, 1);
            PerformMove();
        }

        public void AnimateRectangle()
        {
            TranslateTransform animatedTranslateTransform =
                new TranslateTransform();

            string animationName = "AnimatedTranslateTransform_" + moves_taken + "_" + run_number;
            this.RegisterName(animationName, animatedTranslateTransform);

            curr.RenderTransform = animatedTranslateTransform;

            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(0, 0);
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();
            pBezierSegment.Points.Add(new Point(0, y_offset_up));
            pBezierSegment.Points.Add(new Point(x_offset, y_offset_up));
            pBezierSegment.Points.Add(new Point(x_offset, y_offset_down));

            pFigure.Segments.Add(pBezierSegment);
            animationPath.Figures.Add(pFigure);

            animationPath.Freeze();

            DoubleAnimationUsingPath translateXAnimation =
                new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = animationPath;
            translateXAnimation.Duration = TimeSpan.FromSeconds(animation_speed);

            translateXAnimation.Source = PathAnimationSource.X;

            Storyboard.SetTargetName(translateXAnimation, animationName);
            Storyboard.SetTargetProperty(translateXAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            DoubleAnimationUsingPath translateYAnimation =
                new DoubleAnimationUsingPath();
            translateYAnimation.PathGeometry = animationPath;
            translateYAnimation.Duration = TimeSpan.FromSeconds(animation_speed);

            translateYAnimation.Source = PathAnimationSource.Y;

            translateXAnimation.FillBehavior = FillBehavior.Stop;
            translateYAnimation.FillBehavior = FillBehavior.Stop;

            Storyboard.SetTargetName(translateYAnimation, animationName);
            Storyboard.SetTargetProperty(translateYAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);
            pathAnimationStoryboard.Completed += EndAnimateRectangle;
            pathAnimationStoryboard.Begin(this, true);
        }

        public void EndAnimateRectangle(object sender, EventArgs e)
        {
            Canvas.SetLeft(curr, Canvas.GetLeft(curr) + x_offset);
            Canvas.SetTop(curr, Canvas.GetTop(curr) + y_offset_down);
            moves_taken++;
            PerformMove();
        }

        public void ResetPuzzle()
        {
            AbortAnimation();
            textBx_moves.Clear();
            btn_start.Visibility = Visibility.Visible;
            mnu_start.IsEnabled = true;
            btn_pause.Visibility = Visibility.Hidden;
            mnu_pause.IsEnabled = false;
            btn_abort.Visibility = Visibility.Hidden;
            mnu_abort.IsEnabled = false;
            lbl_numDisks.Visibility = Visibility.Visible;
            slider_numDisks.Visibility = Visibility.Visible;
            lbl_animationSpeed.Visibility = Visibility.Visible;
            radioBtn_slow.Visibility = Visibility.Visible;
            radioBtn_normal.Visibility = Visibility.Visible;
            radioBtn_fast.Visibility = Visibility.Visible;
            lbl_0.Visibility = Visibility.Hidden;
            lbl_1.Visibility = Visibility.Hidden;
            lbl_2.Visibility = Visibility.Hidden;
            lbl_moves.Visibility = Visibility.Hidden;
            textBx_moves.Visibility = Visibility.Hidden;
            rect_peg0_base.Visibility = Visibility.Hidden;
            rect_peg1_base.Visibility = Visibility.Hidden;
            rect_peg2_base.Visibility = Visibility.Hidden;
            rect_peg0_spool.Visibility = Visibility.Hidden;
            rect_peg1_spool.Visibility = Visibility.Hidden;
            rect_peg2_spool.Visibility = Visibility.Hidden;
            for (int i = 0; i < 8; i++)
            {
                all_rects[i].Visibility = Visibility.Hidden;
            }

        }

        public void AbortAnimation()
        {
            pathAnimationStoryboard.Stop(this);

        }
        
        private void slider_numDisks_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            num_pegs = (int)slider_numDisks.Value;
            lbl_numDisks.Content = "Number of Disks: " + num_pegs;
        }
        
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            StartPuzzle();
        }

        private void radioBtn_slow_Checked(object sender, RoutedEventArgs e)
        {
            animation_speed = 3.0;
        }

        private void radioBtn_normal_Checked(object sender, RoutedEventArgs e)
        {
            animation_speed = 1.0;
        }

        private void radioBtn_fast_Checked(object sender, RoutedEventArgs e)
        {
            animation_speed = 0.2;
        }

        private void btn_abort_Click(object sender, RoutedEventArgs e)
        {
            ResetPuzzle();     
        }

        private void mnu_start_Click(object sender, RoutedEventArgs e)
        {
            StartPuzzle();
        }

        private void mnu_abort_Click(object sender, RoutedEventArgs e)
        {
            ResetPuzzle();
        }

        private void mnu_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mnu_howToPlay_Click(object sender, RoutedEventArgs e)
        {
            string message = "Choose the number of disks you would like to animate using the slider bar.\n\n"
                           + "Next choose an animation speed using the radio buttons.\n\n"
                           + "Finally click 'Start' to begin the animation.\n\n"
                           + "Moves are displayed in a text box on the right side of the window.\n\n"
                           + "The peg numbers for the moves are labeled on the base of each peg.\n\n"
                           + "Click 'Pause' to pause the animation.\n\n"
                           + "Click 'Resume to resume a paused animation.\n\n"
                           + "Click 'Abort' while the animation is running to return to the setup screen.";
            MessageBox.Show(message, "How to Play");
        }

        private void mnu_about_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Matthew Kerr\n"
                       + "Eastern Washington University\n"
                       + "Winter 2015\n\n"
                       + "Version 1.0.0.0\n"
                       + ".NET Framework v4.5\n"
                       + "Compiled for 32-bit systems";
            MessageBox.Show(msg, "About");
        }

        private void btn_pause_Click(object sender, RoutedEventArgs e)
        {
            if (!paused)
            {
                pathAnimationStoryboard.Pause(this);
                paused = true;
                btn_pause.Content = "Resume";
                mnu_pause.Header = "Resume";
            }
            else
            {
                pathAnimationStoryboard.Resume(this);
                paused = false;
                btn_pause.Content = "Pause";
                mnu_pause.Header = "Pause";
            }
        }

        private void mnu_pause_Click(object sender, RoutedEventArgs e)
        {
            if (!paused)
            {
                pathAnimationStoryboard.Pause(this);
                paused = true;
                btn_pause.Content = "Resume";
                mnu_pause.Header = "Resume";
            }
            else
            {
                pathAnimationStoryboard.Resume(this);
                paused = false;
                btn_pause.Content = "Pause";
                mnu_pause.Header = "Pause";
            }
        }
    }
}
