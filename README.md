# Imagebinarizer  [![.NET Core Desktop](https://github.com/UniversityOfAppliedSciencesFrankfurt/imagebinarizer/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/UniversityOfAppliedSciencesFrankfurt/imagebinarizer/actions/workflows/dotnet-desktop.yml)

Performs binarization of images. It converts an image to the 2D representation of 01 bits. Inserting image and the desired configurations, the program will generate a new binary image (1 or 0 represents for each pixels) saved as text file.

## Example:

This is the original logo of daenet:

![](/images/daenet.png)         

If you want to create the binrized representation of the same image you can execute following command:

~~~
command
~~~

This command will create th esame image in binarized form by using threshold values ..... TODO. and save it to the folfder/file...
If you want to change the size of the image you can try follosing command....

~~~
command
~~~

!!!!Use this image as an input and set the custom width to be 120, the Binarized image is generated as below:

```
111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111110011111111111111111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111000011111111111111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111100000001111111111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111110000000001111111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111110000000000001111111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111110000000000000001111111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111110000000000000000001111111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111100000000000000000000001111111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111100000000000000000000000001111111111111111111111111111111111111111111111111111111111111
111111111111111111111111111111111000000000000000000000000000001111111111111111111111111111111111111111111111111111100001
111111111111111111111111111111110000000000000111000000000000000001111111111111111111111111111111111111111111111100011001
111111111111111111111111111111100000000000001110000000000000000000000000000111110000000000001111111000000001110001111000
111111111111111111111111111111000000111111101110000011111111000000011111111001110011111111110011001111111100110011111100
111111111111111111111111111100000001110000011100000000000011100000111000011100110111100001110010011100001110010001110001
111111111111111111111111110000000011100000011100000011111111100001111111111100100111001001110110111111111110011011100111
111111111111111111111111100000000011100000111000001110000111000001110000000000001110011011100100111000000000010011101111
111111111111111111111110000000000011110001111000001110001111000000111000111000001110010011100110011100011110110011100001
111111111111111111111000000000000000111110111000000111110111000000011111110000001110000111001111001111111001110011110011
111111111111111111100000000000000000000000000000000000000000000000000000000000000000000000001111110000001111111100000111
111111111111111110000000000000000000000000000001111111111111111111111111000000000000000000000111111111111111111111111111
111111111111111000000000000000000000011111111111111111111111111111111111111111111000000000000000011111111111111111111111
111111111111000000000000000001111111111111111111111111111111111111111111111111111111111110000000000011111111111111111111
111111111100000000000001111111111111111111111111111111111111111111111111111111111111111111111110000000011111111111111111
111111110000000011111111111111111111111111111111111111111111111111111111111111111111111111111111111100000011111111111111
111110000001111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111000011111111111
111000111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111110011111111
111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111011111
```


This binarize image then can be used for many purposes, such as being used in console application:

![](/images/console.png)

### Inverted Image
Another option to binarize the image is to get the inverse of it. The pictures below compare the normal binarized image (left) with the inverted binarized image (right):

<img src="/images/NormalConvert.png" width="400"><img src="/images/InverseConvert.png" width="400">

### Gray Scale

Describe which issues we have with binarization. Then introduce gray-scale. Excplain how gray-scale can solve the issue.

There are more interesting features (WHY it is intersting) to look into in this project: width and height of image customing, gray scale binarizing, color recognizing with thresholds setup, ...

Even image contour recognition can also be done with some tries in thresholds setup as shown below.

<img src="/images/Contour.png">
