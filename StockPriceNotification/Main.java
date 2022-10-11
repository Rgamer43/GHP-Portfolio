import com.gargoylesoftware.htmlunit.BrowserVersion;
import com.gargoylesoftware.htmlunit.Page;
import com.gargoylesoftware.htmlunit.SilentCssErrorHandler;
import com.gargoylesoftware.htmlunit.WebClient;
import com.gargoylesoftware.htmlunit.html.HtmlPage;
import com.gargoylesoftware.htmlunit.html.HtmlSpan;
import com.gargoylesoftware.htmlunit.javascript.SilentJavaScriptErrorListener;
import org.w3c.dom.html.HTMLElement;

import javax.mail.*;
import javax.mail.internet.AddressException;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import java.awt.*;
import java.io.IOException;
import java.net.MalformedURLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;
import javax.activation.*;

public class Main {

    static Stock[] watchlist = new Stock[] {
            new Stock("FXAIX"),
            new Stock("FDVLX"),
            new Stock("FDSVX")
    };

    public static void main(String[] args) throws InterruptedException {
        WebClient webClient;
        boolean sendUpdate = false;

        System.out.println("Beginning loop...");

        while(true) {
            System.out.println("Looping...");

            try {
                for(int i = 0; i < watchlist.length; i++) {
                    System.out.println("Beginning watchlist #" + i);

                    System.out.println("Initting webClient...");
                    webClient = new WebClient(BrowserVersion.EDGE);

                    webClient.getOptions().setCssEnabled(false);
                    webClient.getOptions().setThrowExceptionOnScriptError(false);
                    webClient.getOptions().setThrowExceptionOnFailingStatusCode(false);
                    webClient.getOptions().setPrintContentOnFailingStatusCode(false);
                    webClient.setJavaScriptErrorListener(new SilentJavaScriptErrorListener());
                    webClient.setCssErrorHandler(new SilentCssErrorHandler());
                    System.out.println("Initted webClient");

                    Stock stock = watchlist[i];
                    String source = "https://finance.yahoo.com/quote/" + stock.name + "/";

                    System.out.println("Scraping " + stock.name);
                    HtmlPage page = webClient.getPage(source);
                    page = (HtmlPage) page.refresh();

                    System.out.println("About to scrape by Xpath");
                    com.gargoylesoftware.htmlunit.html.HtmlUnknownElement results =
                                    page.getFirstByXPath("//fin-streamer[@data-field='regularMarketChangePercent' and @data-symbol='" + stock.name + "']");

                    //System.out.println(page.asXml());

                    System.out.println("Displayed Price: " + results.getTextContent());
                    System.out.println("Prev Price: " + stock.lastChange);


                    if(!stock.lastChange.equals(results.getTextContent())) {
                        System.out.println("New price! " + results.getTextContent() + ", from " +
                                source);
                        stock.lastChange = results.getTextContent();
                        sendUpdate = true;
                    }
                }
            } catch (Exception e) {
                //e.printStackTrace();
                System.out.println("Error caught");
            }

            System.out.println("Finished loop");

            if(sendUpdate) sendEmail();
            else System.out.println("Not sending update");

            sendUpdate = false;

            System.out.println("Waiting...");
            Thread.sleep(1000*60*15);
        }
    }

    static void sendEmail() {
        Properties prop = new Properties();
        prop.setProperty("mail.smtp.host", "smtp.gmail.com");
        prop.setProperty("mail.smtp.port", "587");
        prop.setProperty("mail.smtp.auth", "true");
        prop.setProperty("mail.smtp.starttls.enable", "true");
        prop.setProperty("mail.smtp.starttls.required", "true");
        prop.setProperty("mail.smtp.ssl.protocols", "TLSv1.2");

        Session session = Session.getInstance(prop, new Authenticator() {
            @Override
            protected PasswordAuthentication getPasswordAuthentication() {
                return new PasswordAuthentication("rddellosso4@gmail.com", "Redacted");
            }
        });

        System.out.println("Preparing email...");

        try {
            MimeMessage message = new MimeMessage(session);

            message.setFrom(new InternetAddress("from@gmail.com"));
            message.addRecipient(Message.RecipientType.TO, new InternetAddress("rddellosso4@gmail.com"));

            message.setSubject("Today's Stock Updates");

            String content = "";
            for(int i = 0; i < watchlist.length; i++) content += watchlist[i].name + ": " + watchlist[i].lastChange + "\n";
            message.setText(content);

            Transport.send(message);
            System.out.println("Sent email");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

}
